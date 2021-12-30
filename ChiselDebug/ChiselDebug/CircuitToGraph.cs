using ChiselDebug.GraphFIR.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using static FIRRTL.Extensions;

namespace ChiselDebug
{
    internal class VisitHelper
    {
        public readonly Module Mod;
        private readonly CircuitGraph LowFirGraph;
        public readonly Dictionary<string, FIRRTL.DefModule> ModuleRoots;
        public readonly bool IsConditionalModule;
        private readonly VisitHelper ParentHelper;
        private readonly VisitHelper RootHelper;

        private readonly Stack<GraphFIR.IO.Output> ScopeEnabledConditions = new Stack<GraphFIR.IO.Output>();
        public GraphFIR.IO.Output ScopeEnabledCond 
        { 
            get
            {
                if (ScopeEnabledConditions.Count == 0)
                {
                    ConstValue constEnabled = new ConstValue(new FIRRTL.UIntLiteral(1, 1));
                    AddNodeToModule(constEnabled);

                    ScopeEnabledConditions.Push(constEnabled.Result);
                }

                return ScopeEnabledConditions.Peek();
            }
        }

        public VisitHelper(Module mod, CircuitGraph lowFirGraph) : this(mod, lowFirGraph, new Dictionary<string, FIRRTL.DefModule>(), null, false, null)
        { }

        private VisitHelper(Module mod, CircuitGraph lowFirGraph, Dictionary<string, FIRRTL.DefModule> roots, VisitHelper parentHelper, bool isConditional, VisitHelper rootHelper)
        {
            this.Mod = mod;
            this.LowFirGraph = lowFirGraph;
            this.ModuleRoots = roots;
            this.ParentHelper = parentHelper;
            this.IsConditionalModule = isConditional;
            this.RootHelper = rootHelper;
        }

        public VisitHelper ForNewModule(string moduleName, string instanceName, FIRRTL.DefModule moduleDef)
        {
            return new VisitHelper(new GraphFIR.Components.Module(moduleName, instanceName, Mod, moduleDef), LowFirGraph, ModuleRoots, this, false, RootHelper ?? this);
        }

        public VisitHelper ForNewCondModule(string moduleName, FIRRTL.DefModule moduleDef)
        {
            return new VisitHelper(new Module(moduleName, null, Mod, moduleDef), LowFirGraph, ModuleRoots, this, true, RootHelper ?? this);
        }

        public void AddNodeToModule(FIRRTLNode node)
        {
            Mod.AddNode(node);
        }

        public void EnterEnabledScope(GraphFIR.IO.Output enableCond)
        {
            ScopeEnabledConditions.Push(enableCond);
        }

        public void ExitEnabledScope()
        {
            ScopeEnabledConditions.Pop();
        }

        private string GetActualModuleName()
        {
            VisitHelper helper = this;
            while (helper.IsConditionalModule)
            {
                helper = helper.ParentHelper;
            }

            return helper.Mod.Name;
        }

        private string[] GetPathToCurrentActualModule()
        {
            List<string> path = new List<string>();

            VisitHelper helper = this;
            while (helper.Mod != null)
            {
                if (!helper.IsConditionalModule)
                {
                    path.Add(helper.Mod.Name);
                }

                helper = helper.ParentHelper;
            }

            //Path is from node to root but want from root to node
            //which is why the path is reversed
            path.Reverse();

            return path.ToArray();
        }
        
        public bool HasLowFirGraph()
        {
            return LowFirGraph != null;
        }

        public bool IsHighFirGrapth()
        {
            return HasLowFirGraph();
        }

        public FIRRTL.FirrtlNode GetDefNodeFromLowFirrtlGraph(string nodeName)
        {
            Module lowFirMod = LowFirGraph.MainModule;

            string[] pathToModule = GetPathToCurrentActualModule();

            //Skip first module name as it's the name of the root node
            //that we start with
            foreach (var pathModName in pathToModule.Skip(1))
            {
                FIRRTLNode[] lowModNodes = lowFirMod.GetAllNodes();
                FIRRTLNode childLowModNode = lowModNodes.FirstOrDefault(x => x is Module mod && mod.Name == pathModName);
                if (childLowModNode == null)
                {
                    throw new Exception("High level firrtl module path didn't match low level firrtl module path.");
                }

                lowFirMod = (Module)childLowModNode;
            }

            //This is a meh way of going about getting the correct node.
            //Nodes by themselves don't have a name so it works on the assumption
            //that there exists an io with the name which points to the correct node.
            GraphFIR.IO.FIRIO nodeIO = (GraphFIR.IO.FIRIO)lowFirMod.GetIO(nodeName);
            return nodeIO.Flatten().First().Node.FirDefNode;
        }

        private long UniqueNumber = 0;

        internal string GetUniqueName()
        {
            if (RootHelper != null)
            {
                return RootHelper.GetUniqueName();
            }
            else
            {
                return $"~{UniqueNumber++}";
            }
        }
    }

    public static class CircuitToGraph
    {
        public static CircuitGraph GetAsGraph(FIRRTL.Circuit circuit, CircuitGraph graphLowFir = null)
        {
            VisitHelper helper = new VisitHelper(null, graphLowFir);
            foreach (var moduleDef in circuit.Modules)
            {
                helper.ModuleRoots.Add(moduleDef.Name, moduleDef);
            }

            FIRRTL.DefModule mainModDef = circuit.Modules.SingleOrDefault(x => x.Name == circuit.Main);
            if (mainModDef == null)
            {
                throw new ChiselDebugException("Circuit does not contain a module with the circuits name.");
            }
            Module mainModule = VisitModule(helper, null, mainModDef);
            GraphFIR.IO.AggregateConnections.ConnectionAllAggregateIO(mainModule);
            foreach (var mod in mainModule.GetAllNestedNodesOfType<Module>())
            {
                CleanupModule(mod);
            }

            return new CircuitGraph(circuit.Main, mainModule);
        }

        private static Module VisitModule(VisitHelper parentHelper, string moduleInstanceName, FIRRTL.DefModule moduleDef)
        {
            if (moduleDef is FIRRTL.Module mod)
            {
                VisitHelper helper = parentHelper.ForNewModule(mod.Name, moduleInstanceName, mod);
                foreach (var port in mod.Ports)
                {
                    VisitPort(helper, port);
                }

                VisitStatement(helper, mod.Body);

                return helper.Mod;
            }
            else if (moduleDef is FIRRTL.ExtModule extMod)
            {
                VisitHelper helper = parentHelper.ForNewModule(extMod.Name, moduleInstanceName, extMod);
                foreach (var port in extMod.Ports)
                {
                    VisitPort(helper, port);
                }

                return helper.Mod;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void CleanupModule(Module mod)
        {
            //In a truely stupid move, FIRRTL supports connecting
            //Sinks to other sinks. In order to support that case
            //a sink can pretend to be a source. It's important 
            //that they stop pretending after the module graph
            //has been made because this hack shouldn't be
            //visible outside of graph creation. Everything else
            //should still work on the assumption that only
            //connections from a source to a sink are possible.
            mod.RemoveAllDuplexWires();

            if (!mod.IsConditional)
            {
                GraphFIR.IO.IOHelper.BypassCondConnectionsThroughCondModules(mod);
            }
        }

        private static void VisitPort(VisitHelper helper, FIRRTL.Port port)
        {
            var io = VisitType(helper, port.Direction, port.Name, port.Type, false);
            helper.Mod.AddExternalIO(io.Copy(helper.Mod));
        }

        private static GraphFIR.IO.FIRIO VisitTypeAsPassive(VisitHelper helper, FIRRTL.Dir direction, string name, FIRRTL.IFIRType type)
        {
            return VisitType(helper, direction, name, type, true);
        }

        private static GraphFIR.IO.FIRIO VisitType(VisitHelper helper, FIRRTL.Dir direction, string name, FIRRTL.IFIRType type, bool forcePassive)
        {
            if (type is FIRRTL.BundleType bundle)
            {
                return VisitBundle(helper, direction, name, bundle, forcePassive);
            }
            else if (type is FIRRTL.VectorType vec)
            {
                return VisitVector(helper, direction, name, vec, forcePassive);
            }
            else if (direction == FIRRTL.Dir.Input)
            {
                return new GraphFIR.IO.Input(null, name, type);
            }
            else if (direction == FIRRTL.Dir.Output)
            {
                return new GraphFIR.IO.Output(null, name, type);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static GraphFIR.IO.FIRIO VisitBundle(VisitHelper helper, FIRRTL.Dir direction, string bundleName, FIRRTL.BundleType bundle, bool forcePassive)
        {
            List<GraphFIR.IO.FIRIO> io = new List<GraphFIR.IO.FIRIO>();
            foreach (var field in bundle.Fields)
            {
                //If passive then ignore flips so all flows are the same
                FIRRTL.Dir fieldDir = !forcePassive ? direction.Flip(field.Flip) : direction;
                io.Add(VisitType(helper, fieldDir, field.Name, field.Type, forcePassive));
            }

            return new GraphFIR.IO.IOBundle(null, bundleName, io);
        }

        private static GraphFIR.IO.FIRIO VisitVector(VisitHelper helper, FIRRTL.Dir direction, string vectorName, FIRRTL.VectorType vec, bool forcePassive)
        {
            var type = VisitType(helper, direction, null, vec.Type, forcePassive);
            return new GraphFIR.IO.Vector(null, vectorName, vec.Size, type);
        }

        private static void VisitStatement(VisitHelper helper, FIRRTL.Statement statement)
        {
            if (statement is FIRRTL.EmptyStmt)
            {
                return;
            }
            else if (statement is FIRRTL.Block block)
            {
                for (int i = 0; i < block.Statements.Count; i++)
                {
                    VisitStatement(helper, block.Statements[i]);
                }
            }
            else if (statement is FIRRTL.Conditionally conditional)
            {
                VisitConditional(helper, conditional);
            }
            else if (statement is FIRRTL.Stop stop)
            {
                var clock = (GraphFIR.IO.Output)VisitExp(helper, stop.Clk, GraphFIR.IO.FlowChange.Source);
                var enable = (GraphFIR.IO.Output)VisitExp(helper, stop.Enabled, GraphFIR.IO.FlowChange.Source);

                var firStop = new FirStop(clock, enable, stop.Ret, stop);
                helper.AddNodeToModule(firStop);
            }
            else if (statement is FIRRTL.Attach)
            {
                return;
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.Print)
            {
                return;
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.Verification)
            {
                return;
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.Connect connect)
            {
                VisitConnect(helper, connect.Expr, connect.Loc, false);
            }
            else if (statement is FIRRTL.PartialConnect parConnected)
            {
                VisitConnect(helper, parConnected.Expr, parConnected.Loc, true);
            }
            else if (statement is FIRRTL.IsInvalid)
            {
                return;
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.CDefMemory cmem)
            {
                //If have access to low firrth graph then get memory definition
                //from it as it includes all port definitions. This avoids having
                //to infer memory port types.
                if (helper.HasLowFirGraph())
                {
                    var lowFirMem = (FIRRTL.DefMemory)helper.GetDefNodeFromLowFirrtlGraph(cmem.Name);
                    VisitStatement(helper, lowFirMem);

                    //Low level firrtl addresses the ports through the memory but
                    //high level firrtl directly addreses the ports. Need to
                    //make the ports directly addresseable which is why this is done.
                    var lowMem = (GraphFIR.IO.MemoryIO)helper.Mod.GetIO(cmem.Name);
                    foreach (GraphFIR.IO.MemPort port in lowMem.GetAllPorts())
                    {
                        port.FromHighLevelFIRRTL = true;
                        helper.Mod.AddMemoryPort(port);
                    }
                }
                else
                {
                    GraphFIR.IO.FIRIO inputType = VisitTypeAsPassive(helper, FIRRTL.Dir.Input, null, cmem.Type);
                    var memory = new Memory(cmem.Name, inputType, cmem.Size, 0, 0, cmem.Ruw, cmem);

                    helper.AddNodeToModule(memory);
                }
            }
            else if (statement is FIRRTL.CDefMPort memPort)
            {
                var memory = (GraphFIR.IO.MemoryIO)helper.Mod.GetIO(memPort.Mem);

                //Port may already have been created if the memory used the low firrtl
                //memory definition which contain all ports that will be used
                GraphFIR.IO.MemPort port;
                if (memory.TryGetIO(memPort.Name, out var existingPort))
                {
                    port = (GraphFIR.IO.MemPort)existingPort;
                }
                else
                {
                    port = memPort.Direction switch
                    {
                        FIRRTL.MPortDir.MInfer => throw new NotImplementedException(),
                        FIRRTL.MPortDir.MRead => memory.AddReadPort(memPort.Name),
                        FIRRTL.MPortDir.MWrite => memory.AddWritePort(memPort.Name),
                        FIRRTL.MPortDir.MReadWrite => memory.AddReadWritePort(memPort.Name),
                        var error => throw new Exception($"Unknown memory port type. Type: {error}")
                    };

                    port.FromHighLevelFIRRTL = true;
                    helper.Mod.AddMemoryPort(port);
                }

                ConnectIO(helper, VisitExp(helper, memPort.Exps[0], GraphFIR.IO.FlowChange.Source), port.Address, false);
                ConnectIO(helper, VisitExp(helper, memPort.Exps[1], GraphFIR.IO.FlowChange.Source), port.Clock, false, false);
                ConnectIO(helper, helper.ScopeEnabledCond, port.Enabled, false);

                //if port has mask then by default set whole mask to true
                if (port.HasMask())
                {
                    GraphFIR.IO.FIRIO mask = port.GetMask();
                    GraphFIR.IO.Output const1 = (GraphFIR.IO.Output)VisitExp(helper, new FIRRTL.UIntLiteral(0, 1), GraphFIR.IO.FlowChange.Source);
                    foreach (var maskInput in mask.Flatten())
                    {
                        ConnectIO(helper, const1, maskInput, false);
                    }
                }
            }
            else if (statement is FIRRTL.DefWire defWire)
            {
                GraphFIR.IO.FIRIO inputType = VisitTypeAsPassive(helper, FIRRTL.Dir.Output, null, defWire.Type);
                inputType = inputType.ToFlow(GraphFIR.IO.FlowChange.Sink, null);
                Wire wire = new Wire(defWire.Name, inputType, defWire);

                helper.AddNodeToModule(wire);
            }
            else if (statement is FIRRTL.DefRegister reg)
            {
                GraphFIR.IO.Output clock = (GraphFIR.IO.Output)VisitExp(helper, reg.Clock, GraphFIR.IO.FlowChange.Source);
                GraphFIR.IO.Output reset = null;
                GraphFIR.IO.FIRIO initValue = null;

                if (reg.HasResetAndInit())
                {
                    reset = (GraphFIR.IO.Output)VisitExp(helper, reg.Reset, GraphFIR.IO.FlowChange.Source);
                    initValue = VisitExp(helper, reg.Init, GraphFIR.IO.FlowChange.Source);
                }

                GraphFIR.IO.FIRIO inputType = VisitTypeAsPassive(helper, FIRRTL.Dir.Input, null, reg.Type);
                Register register = new Register(reg.Name, inputType, clock, reset, initValue, reg);
                helper.AddNodeToModule(register);
            }
            else if (statement is FIRRTL.DefInstance instance)
            {
                Module mod = VisitModule(helper, instance.Name, helper.ModuleRoots[instance.Module]);
                helper.AddNodeToModule(mod);
            }
            else if (statement is FIRRTL.DefNode node)
            {
                var nodeOut = VisitExp(helper, node.Value, GraphFIR.IO.FlowChange.Source);

                if (node.Value is not FIRRTL.RefLikeExpression)
                {
                    nodeOut.SetName(node.Name);
                }

                helper.Mod.AddIORename(node.Name, nodeOut);
            }
            else if (statement is FIRRTL.DefMemory mem)
            {
                GraphFIR.IO.FIRIO inputType = VisitTypeAsPassive(helper, FIRRTL.Dir.Input, null, mem.Type);
                var memory = new Memory(mem.Name, inputType, mem.Depth, mem.ReadLatency, mem.WriteLatency, mem.Ruw, mem);

                foreach (var portName in mem.Readers)
                {
                    memory.AddReadPort(portName);
                }
                foreach (var portName in mem.Writers)
                {
                    memory.AddWritePort(portName);
                }
                foreach (var portName in mem.ReadWriters)
                {
                    memory.AddReadWritePort(portName);
                }

                helper.AddNodeToModule(memory);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void VisitConnect(VisitHelper helper, FIRRTL.Expression exprFrom, FIRRTL.Expression exprTo, bool isPartial)
        {
            GraphFIR.IO.FIRIO from = VisitExp(helper, exprFrom, GraphFIR.IO.FlowChange.Source);
            GraphFIR.IO.FIRIO to = (GraphFIR.IO.FIRIO)VisitRef(helper, exprTo, helper.Mod, GraphFIR.IO.FlowChange.Sink);

            //Can only connect two aggregates. If any of the two are not an
            //aggregate type then try convert both to scalar io and connect them.
            if (from is not GraphFIR.IO.AggregateIO || to is not GraphFIR.IO.AggregateIO)
            {
                from = from.GetOutput();
                to = to.GetInput();
            }

            ConnectIO(helper, from, to, isPartial);
        }

        private static void ConnectIO(VisitHelper helper, GraphFIR.IO.FIRIO from, GraphFIR.IO.FIRIO to, bool isPartial, bool canBeConditional = true)
        {
            Module fromMod = from.GetModResideIn();
            Module toMod = to.GetModResideIn();

            //If going from inside to outside or outside to outside
            //then add condition to that connection if currently in
            //conditional module.
            GraphFIR.IO.Output condition = null;
            if (canBeConditional &&
                ((fromMod == helper.Mod && toMod != helper.Mod) ||
                 (fromMod != helper.Mod && toMod != helper.Mod)))
            {
                condition = helper.Mod.EnableCon;
            }

            from.ConnectToInput(to, isPartial, false, condition);

            //If writing to a memory ports data in high level firrtl, then
            //the mask also has to be set to true for the part of the port data
            //that was written to.
            if (GraphFIR.IO.IOHelper.TryGetParentMemPort(to, out var memPort) && 
                memPort.FromHighLevelFIRRTL &&
                GraphFIR.IO.IOHelper.IsIOInMaskableMemPortData(to, memPort))
            {
                var scopeEnableCond = helper.ScopeEnabledCond;
                foreach (GraphFIR.IO.Input dataInputWrittenTo in to.Flatten())
                {
                    var dataInputMask = memPort.GetMaskFromDataInput(dataInputWrittenTo);
                    scopeEnableCond.ConnectToInput(dataInputMask, false, false, scopeEnableCond);
                }
            }
        }

        private static void VisitConditional(VisitHelper parentHelper, FIRRTL.Conditionally conditional)
        {
            Conditional cond = new Conditional(conditional);

            void AddCondModule(GraphFIR.IO.Output ena, FIRRTL.Statement body)
            {
                VisitHelper helper = parentHelper.ForNewCondModule(parentHelper.GetUniqueName(), null);

                var internalEnaDummy = new DummyPassthrough(ena);
                var internalUseEna = new DummySink(internalEnaDummy.Result);
                helper.AddNodeToModule(internalEnaDummy);
                helper.AddNodeToModule(internalUseEna);
                helper.Mod.SetEnableCond(internalEnaDummy.Result);

                //Set signal that enables this scope as things like memory
                //ports need it
                helper.EnterEnabledScope(internalEnaDummy.Result);

                //Fill out module
                VisitStatement(helper, body);

                cond.AddConditionalModule(helper.Mod);

                helper.ExitEnabledScope();
            }

            GraphFIR.IO.Output enableCond = (GraphFIR.IO.Output)VisitExp(parentHelper, conditional.Pred, GraphFIR.IO.FlowChange.Source);

            if (conditional.HasIf())
            {
                GraphFIR.IO.Output ifEnableCond = enableCond;
                if (parentHelper.Mod.IsConditional)
                {
                    FIRAnd chainConditions = new FIRAnd(parentHelper.Mod.EnableCon, enableCond, new FIRRTL.UIntType(1), null);
                    parentHelper.AddNodeToModule(chainConditions);

                    ifEnableCond = chainConditions.Result;
                }

                AddCondModule(ifEnableCond, conditional.WhenTrue);
            }
            if (conditional.HasElse())
            {
                FIRNot notEnableComponent = new FIRNot(enableCond, new FIRRTL.UIntType(1), null);
                parentHelper.AddNodeToModule(notEnableComponent);

                GraphFIR.IO.Output elseEnableCond = notEnableComponent.Result;
                if (parentHelper.Mod.IsConditional)
                {
                    FIRAnd chainConditions = new FIRAnd(parentHelper.Mod.EnableCon, elseEnableCond, new FIRRTL.UIntType(1), null);
                    parentHelper.AddNodeToModule(chainConditions);

                    elseEnableCond = chainConditions.Result;
                }

                AddCondModule(elseEnableCond, conditional.Alt);
            }

            parentHelper.AddNodeToModule(cond);
        }

        private static GraphFIR.IO.FIRIO VisitExp(VisitHelper helper, FIRRTL.Expression exp, GraphFIR.IO.FlowChange ioFlow)
        {
            if (exp is FIRRTL.RefLikeExpression)
            {
                return (GraphFIR.IO.FIRIO)VisitRef(helper, exp, helper.Mod, ioFlow);
            }

            if (exp is FIRRTL.Literal lit)
            {
                ConstValue value = new ConstValue(lit);

                helper.AddNodeToModule(value);
                return value.Result;
            }
            else if (exp is FIRRTL.DoPrim prim)
            {
                var args = prim.Args.Select(x => VisitExp(helper, x, GraphFIR.IO.FlowChange.Source)).Cast<GraphFIR.IO.Output>().ToArray();
                FIRRTLPrimOP nodePrim;
                if (prim.Op is FIRRTL.Add)
                {
                    nodePrim = new FIRAdd(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Sub)
                {
                    nodePrim = new FIRSub(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Mul)
                {
                    nodePrim = new FIRMul(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Div)
                {
                    nodePrim = new FIRDiv(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Rem)
                {
                    nodePrim = new FIRRem(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Dshl)
                {
                    nodePrim = new FIRDshl(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Dshr)
                {
                    nodePrim = new FIRDshr(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Cat)
                {
                    nodePrim = new FIRCat(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Eq)
                {
                    nodePrim = new FIREq(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Neq)
                {
                    nodePrim = new FIRNeq(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Geq)
                {
                    nodePrim = new FIRGeq(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Leq)
                {
                    nodePrim = new FIRLeq(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Gt)
                {
                    nodePrim = new FIRGt(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Lt)
                {
                    nodePrim = new FIRLt(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.And)
                {
                    nodePrim = new FIRAnd(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Or)
                {
                    nodePrim = new FIROr(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Xor)
                {
                    nodePrim = new FIRXor(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Head)
                {
                    nodePrim = new Head(args[0], prim.Type, (int)prim.Consts[0], prim);
                }
                else if (prim.Op is FIRRTL.Tail)
                {
                    nodePrim = new Tail(args[0], prim.Type, (int)prim.Consts[0], prim);
                }
                else if (prim.Op is FIRRTL.Bits)
                {
                    nodePrim = new BitExtract(args[0], prim.Type, (int)prim.Consts[1], (int)prim.Consts[0], prim);
                }
                else if (prim.Op is FIRRTL.Pad)
                {
                    nodePrim = new Pad(args[0], prim.Type, (int)prim.Consts[0], prim);
                }
                else if (prim.Op is FIRRTL.AsUInt)
                {
                    nodePrim = new FIRAsUInt(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.AsSInt)
                {
                    nodePrim = new FIRAsSInt(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.AsClock)
                {
                    nodePrim = new FIRAsClock(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Cvt)
                {
                    nodePrim = new FIRCvt(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Neg)
                {
                    nodePrim = new FIRNeg(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Not)
                {
                    nodePrim = new FIRNot(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Andr)
                {
                    nodePrim = new FIRAndr(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Orr)
                {
                    nodePrim = new FIROrr(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Xorr)
                {
                    nodePrim = new FIRXorr(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Shl)
                {
                    var constLit = new FIRRTL.UIntLiteral(prim.Consts[0], (int)prim.Consts[0].GetBitLength());
                    var constOutput = (GraphFIR.IO.Output)VisitExp(helper, constLit, GraphFIR.IO.FlowChange.Source);
                    nodePrim = new FIRShl(args[0], constOutput, prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Shr)
                {
                    var constLit = new FIRRTL.UIntLiteral(prim.Consts[0], (int)prim.Consts[0].GetBitLength());
                    var constOutput = (GraphFIR.IO.Output)VisitExp(helper, constLit, GraphFIR.IO.FlowChange.Source);
                    nodePrim = new FIRShr(args[0], constOutput, prim.Type, prim);
                }
                else
                {
                    throw new NotImplementedException();
                }

                helper.AddNodeToModule(nodePrim);
                return nodePrim.Result;
            }
            else if (exp is FIRRTL.Mux mux)
            {
                var cond = (GraphFIR.IO.Output)VisitExp(helper, mux.Cond, GraphFIR.IO.FlowChange.Source);
                var ifTrue = VisitExp(helper, mux.TrueValue, GraphFIR.IO.FlowChange.Source);
                var ifFalse = VisitExp(helper, mux.FalseValue, GraphFIR.IO.FlowChange.Source);

                Mux node = new Mux(new List<GraphFIR.IO.FIRIO>() { ifTrue, ifFalse }, cond, mux);

                helper.AddNodeToModule(node);
                return node.Result;
            }
            else if (exp is FIRRTL.ValidIf validIf)
            {
                var cond = (GraphFIR.IO.Output)VisitExp(helper, validIf.Cond, GraphFIR.IO.FlowChange.Source);
                var ifValid = VisitExp(helper, validIf.Value, GraphFIR.IO.FlowChange.Source);

                Mux node = new Mux(new List<GraphFIR.IO.FIRIO>() { ifValid }, cond, validIf);

                helper.AddNodeToModule(node);
                return node.Result;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static GraphFIR.IO.IContainerIO VisitRef(VisitHelper helper, FIRRTL.Expression exp, GraphFIR.IO.IContainerIO currContainer, GraphFIR.IO.FlowChange ioFlow)
        {
            GraphFIR.IO.IContainerIO refContainer;
            if (exp is FIRRTL.Reference reference)
            {
                refContainer = currContainer.GetIO(reference.Name);
            }
            else if (exp is FIRRTL.SubField subField)
            {
                var subContainer = VisitExp(helper, subField.Expr, ioFlow);
                refContainer = subContainer.GetIO(subField.Name);
            }
            else if (exp is FIRRTL.SubIndex subIndex)
            {
                var subVec = VisitExp(helper, subIndex.Expr, ioFlow);
                var vec = (GraphFIR.IO.Vector)subVec;

                refContainer = vec.GetIndex(subIndex.Value);
            }
            else if (exp is FIRRTL.SubAccess subAccess)
            {
                var subVec = VisitExp(helper, subAccess.Expr, ioFlow);
                var vec = (GraphFIR.IO.Vector)subVec;
                var index = (GraphFIR.IO.Output)VisitExp(helper, subAccess.Index, GraphFIR.IO.FlowChange.Source);

                if (ioFlow == GraphFIR.IO.FlowChange.Source)
                {
                    Mux node = new Mux(vec.GetIOInOrder().ToList(), index, null, true);
                    helper.AddNodeToModule(node);

                    refContainer = node.Result;
                }
                else
                {
                    VectorAssign vecAssign = new VectorAssign(vec, index, helper.Mod.EnableCon, null);
                    helper.AddNodeToModule(vecAssign);

                    refContainer = vecAssign.GetAssignIO();
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            if (refContainer is GraphFIR.IO.MemPort memPort)
            {
                //Memory ports in high level firrtl are acceses in a different
                //way compared to low level firrtl. In high level firrtl, a
                //memory port is treated like a wire connected to its datain/out
                //sub field whereas in low level firrtl the subfield has to be
                //specified.
                if (memPort.FromHighLevelFIRRTL)
                {
                    return GetIOGender(helper, memPort, ioFlow);
                }
            }
            else
            {
                //Never return bigender io. Only this method should have to deal
                //with that mess so the rest of the code doesn't have to.
                //Dealing with it is ugly which is why i want to contain it.
                if (refContainer is GraphFIR.IO.FIRIO firIO)
                {
                    return GetIOGender(helper, firIO, ioFlow);
                }
            }

            return refContainer;
        }

        private static GraphFIR.IO.FIRIO GetIOGender(VisitHelper helper, GraphFIR.IO.FIRIO io, GraphFIR.IO.FlowChange ioFlow)
        {
            if (io is GraphFIR.IO.Input input && ioFlow == GraphFIR.IO.FlowChange.Source)
            {
                string duplexOutputName = helper.Mod.GetDuplexOutputName(input);

                //Try see if it was already created
                if (input.GetModResideIn().TryGetIO(duplexOutputName, out var wireOut))
                {
                    return (GraphFIR.IO.Output)wireOut;
                }

                //Duplex output for this input wasn't created before so make it now.
                //Make it in the module that the input comes from so there won't
                //be multiple duplex inputs residing in different cond modules.
                return input.GetModResideIn().AddDuplexOuputWire(input);
            }

            return io.GetAsFlow(ioFlow);
        }
    }
}
