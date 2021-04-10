using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static FIRRTL.Extensions;

namespace ChiselDebug
{
    internal class VisitHelper
    {
        public readonly GraphFIR.Module Mod;
        private readonly CircuitGraph LowFirGraph;
        public readonly Dictionary<string, FIRRTL.DefModule> ModuleRoots;
        public readonly bool IsConditionalModule;
        private readonly VisitHelper ParentHelper;

        private readonly Stack<GraphFIR.IO.Output> ScopeEnabledConditions = new Stack<GraphFIR.IO.Output>();
        public GraphFIR.IO.Output ScopeEnabledCond 
        { 
            get
            {
                if (ScopeEnabledConditions.Count == 0)
                {
                    GraphFIR.ConstValue constEnabled = new GraphFIR.ConstValue(GetUniqueName(), new FIRRTL.UIntLiteral(1, 1));
                    AddNodeToModule(constEnabled);

                    ScopeEnabledConditions.Push(constEnabled.Result);
                }

                return ScopeEnabledConditions.Peek();
            }
        }

        public VisitHelper(GraphFIR.Module mod, CircuitGraph lowFirGraph) : this(mod, lowFirGraph, new Dictionary<string, FIRRTL.DefModule>(), null, false)
        { }

        private VisitHelper(GraphFIR.Module mod, CircuitGraph lowFirGraph, Dictionary<string, FIRRTL.DefModule> roots, VisitHelper parentHelper, bool isConditional)
        {
            this.Mod = mod;
            this.LowFirGraph = lowFirGraph;
            this.ModuleRoots = roots;
            this.ParentHelper = parentHelper;
            this.IsConditionalModule = isConditional;
        }

        public VisitHelper ForNewModule(string moduleName, FIRRTL.DefModule moduleDef, bool isConditional)
        {
            return new VisitHelper(new GraphFIR.Module(moduleName, moduleDef), LowFirGraph, ModuleRoots, this, isConditional);
        }

        public void AddNodeToModule(GraphFIR.FIRRTLNode node)
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

        public FIRRTL.FirrtlNode GetDefNodeFromLowFirrtlGraph(string nodeName)
        {
            GraphFIR.Module lowFirMod = LowFirGraph.MainModule;

            string[] pathToModule = GetPathToCurrentActualModule();

            //Skip first module name as it's the name of the root node
            //that we start with
            foreach (var pathModName in pathToModule.Skip(1))
            {
                GraphFIR.FIRRTLNode[] lowModNodes = lowFirMod.GetAllNodes();
                GraphFIR.FIRRTLNode childLowModNode = lowModNodes.FirstOrDefault(x => x is GraphFIR.Module mod && mod.Name == pathModName);
                if (childLowModNode == null)
                {
                    throw new Exception("High level firrtl module path didn't match low level firrtl module path.");
                }

                lowFirMod = (GraphFIR.Module)childLowModNode;
            }

            //This is a meh way of going about getting the correct node.
            //Nodes by themselves don't have a name so it works on the assumption
            //that there exists an io with the name which points to the correct node.
            GraphFIR.IO.FIRIO nodeIO = (GraphFIR.IO.FIRIO)lowFirMod.GetIO(nodeName);
            return nodeIO.Flatten().First().Node.FirDefNode;
        }

        private static long UniqueNumber = 0;

        internal string GetUniqueName()
        {
            return $"~{Interlocked.Increment(ref UniqueNumber)}";
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

            FIRRTL.DefModule mainModDef = circuit.Modules.Single(x => x.Name == circuit.Main);
            GraphFIR.Module mainModule = VisitModule(helper, mainModDef);
            return new CircuitGraph(circuit.Main, mainModule);
        }

        private static GraphFIR.Module VisitModule(VisitHelper parentHelper, FIRRTL.DefModule moduleDef)
        {
            if (moduleDef is FIRRTL.Module mod)
            {
                VisitHelper helper = parentHelper.ForNewModule(mod.Name, mod, false);
                foreach (var port in mod.Ports)
                {
                    VisitPort(helper, port);
                }

                VisitStatement(helper, mod.Body);

                CleanupModule(helper);

                return helper.Mod;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void CleanupModule(VisitHelper helper)
        {
            //In a truely stupid move, FIRRTL supports connecting
            //Sinks to other sinks. In order to support that case
            //a sink can pretend to be a source. It's important 
            //that they stop pretending after the module graph
            //has been made because this hack shouldn't be
            //visible outside of graph creation. Everything else
            //should still work on the assumption that only
            //connections from a source to a sink are possible.
            helper.Mod.CorrectIO();

            helper.Mod.RemoveAllWires();
        }

        private static void VisitPort(VisitHelper helper, FIRRTL.Port port)
        {
            foreach (var io in VisitType(helper, port.Direction, port.Name, port.Type))
            {
                helper.Mod.AddExternalIO(io.Copy(helper.Mod));
            }
        }

        private static List<GraphFIR.IO.FIRIO> VisitType(VisitHelper helper, FIRRTL.Dir direction, string name, FIRRTL.IFIRType type)
        {
            List<GraphFIR.IO.FIRIO> io = new List<GraphFIR.IO.FIRIO>();
            if (type is FIRRTL.BundleType bundle)
            {
                io.Add(VisitBundle(helper, direction, name, bundle));
            }
            else if (type is FIRRTL.VectorType vec)
            {
                io.Add(VisitVector(helper, direction, name, vec));
            }
            else if (direction == FIRRTL.Dir.Input)
            {
                io.Add(new GraphFIR.IO.Input(null, name, type));
            }
            else if (direction == FIRRTL.Dir.Output)
            {
                io.Add(new GraphFIR.IO.Output(null, name, type));
            }
            else
            {
                throw new NotImplementedException();
            }

            return io;
        }

        private static GraphFIR.IO.FIRIO VisitBundle(VisitHelper helper, FIRRTL.Dir direction, string bundleName, FIRRTL.BundleType bundle)
        {
            List<GraphFIR.IO.FIRIO> io = new List<GraphFIR.IO.FIRIO>();
            foreach (var field in bundle.Fields)
            {
                FIRRTL.Dir fieldDir = direction.Flip(field.Flip);
                io.AddRange(VisitType(helper, fieldDir, field.Name, field.Type));
            }

            return new GraphFIR.IO.IOBundle(null, bundleName, io);
        }

        private static GraphFIR.IO.FIRIO VisitVector(VisitHelper helper, FIRRTL.Dir direction, string vectorName, FIRRTL.VectorType vec)
        {
            var type = VisitType(helper, direction, string.Empty, vec.Type).Single();
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
                block.Statements.ForEach(x => VisitStatement(helper, x));
            }
            else if (statement is FIRRTL.Conditionally conditional)
            {
                VisitConditional(helper, conditional);
            }
            else if (statement is FIRRTL.Stop)
            {
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.Attach)
            {
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.Print)
            {
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.Verification)
            {
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
                    var lowMem = (GraphFIR.IO.IPortsIO)helper.Mod.GetIO(cmem.Name);
                    foreach (GraphFIR.IO.MemPort port in lowMem.GetAllPorts())
                    {
                        helper.Mod.AddMemoryPort(port);
                    }


                    return;
                }

                GraphFIR.IO.FIRIO inputType = VisitType(helper, FIRRTL.Dir.Input, string.Empty, cmem.Type).Single();
                var memory = new GraphFIR.Memory(cmem.Name, inputType, cmem.Size, 0, 0, cmem.Ruw, cmem);

                helper.Mod.AddMemory(memory);
            }
            else if (statement is FIRRTL.CDefMPort memPort)
            {
                var memory = (GraphFIR.IO.MemoryIO)helper.Mod.GetIO(memPort.Mem);

                //Port may already have been created if the memory used the low firrtl
                //memory definition which contain all ports that will be used
                GraphFIR.IO.MemPort port;
                if (memory.TryGetIO(memPort.Name, false, out var existingPort))
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
                }

                VisitExp(helper, memPort.Exps[0], GraphFIR.IO.IOGender.Male).ConnectToInput(port.Address);
                VisitExp(helper, memPort.Exps[1], GraphFIR.IO.IOGender.Male).ConnectToInput(port.Clock);
                helper.ScopeEnabledCond.ConnectToInput(port.Enabled);

                //if port has mask then by default set whole mask to true
                if (port.HasMask())
                {
                    GraphFIR.IO.FIRIO mask = port.GetMask();
                    GraphFIR.IO.Output const1 = (GraphFIR.IO.Output)VisitExp(helper, new FIRRTL.UIntLiteral(1, 1), GraphFIR.IO.IOGender.Male);
                    foreach (var maskInput in mask.Flatten())
                    {
                        const1.ConnectToInput(maskInput);
                    }
                }

                helper.Mod.AddMemoryPort(port);
            }
            else if (statement is FIRRTL.DefWire defWire)
            {
                GraphFIR.IO.FIRIO inputType = VisitType(helper, FIRRTL.Dir.Output, string.Empty, defWire.Type).Single();
                inputType = inputType.ToFlow(GraphFIR.IO.FlowChange.Sink);
                GraphFIR.Wire wire = new GraphFIR.Wire(defWire.Name, inputType, defWire);

                helper.Mod.AddWire(wire);
            }
            else if (statement is FIRRTL.DefRegister reg)
            {
                GraphFIR.IO.Output clock = (GraphFIR.IO.Output)VisitExp(helper, reg.Clock, GraphFIR.IO.IOGender.Male);
                GraphFIR.IO.Output reset = null;
                GraphFIR.IO.Output initValue = null;

                if (reg.HasResetAndInit())
                {
                    reset = (GraphFIR.IO.Output)VisitExp(helper, reg.Reset, GraphFIR.IO.IOGender.Male);
                    initValue = (GraphFIR.IO.Output)VisitExp(helper, reg.Init, GraphFIR.IO.IOGender.Male);
                }

                GraphFIR.IO.FIRIO inputType = VisitType(helper, FIRRTL.Dir.Input, string.Empty, reg.Type).Single();
                GraphFIR.Register register = new GraphFIR.Register(reg.Name, inputType, clock, reset, initValue, reg);
                helper.Mod.AddRegister(register);
            }
            else if (statement is FIRRTL.DefInstance instance)
            {
                GraphFIR.Module mod = VisitModule(helper, helper.ModuleRoots[instance.Module]);
                helper.Mod.AddModule(mod, instance.Name);
            }
            else if (statement is FIRRTL.DefNode node)
            {
                var nodeOut = VisitExp(helper, node.Value, GraphFIR.IO.IOGender.Male);

                if (node.Value is not FIRRTL.RefLikeExpression)
                {
                    nodeOut.SetName(node.Name);
                }

                helper.Mod.AddIORename(node.Name, nodeOut);
            }
            else if (statement is FIRRTL.DefMemory mem)
            {
                GraphFIR.IO.FIRIO inputType = VisitType(helper, FIRRTL.Dir.Input, string.Empty, mem.Type).Single();
                var memory = new GraphFIR.Memory(mem.Name, inputType, mem.Depth, mem.ReadLatency, mem.WriteLatency, mem.Ruw, mem);

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

                helper.Mod.AddMemory(memory);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void VisitConnect(VisitHelper helper, FIRRTL.Expression exprFrom, FIRRTL.Expression exprTo, bool isPartial)
        {
            GraphFIR.IO.FIRIO from = VisitExp(helper, exprFrom, GraphFIR.IO.IOGender.Male);
            GraphFIR.IO.FIRIO to = (GraphFIR.IO.FIRIO)VisitRef(helper, exprTo, helper.Mod, GraphFIR.IO.IOGender.Female);

            //Can only connect two aggregates. If any of the two are not an
            //aggregate type then try convert both to scalar io and connect them.
            if (from is not GraphFIR.IO.AggregateIO || to is not GraphFIR.IO.AggregateIO)
            {
                from = from.GetOutput();
                to = to.GetInput();
            }

            from.ConnectToInput(to, isPartial);
        }

        private static void VisitConditional(VisitHelper parentHelper, FIRRTL.Conditionally conditional)
        {
            GraphFIR.Conditional cond = new GraphFIR.Conditional(conditional);

            void AddCondModule(GraphFIR.IO.Output ena, FIRRTL.Statement body)
            {
                VisitHelper helper = parentHelper.ForNewModule(parentHelper.GetUniqueName(), null, true);

                //Connect wire that enables condition to module
                GraphFIR.IO.Input enaInput = new GraphFIR.IO.Input(helper.Mod, new FIRRTL.UIntType(1));
                string enaName = "ena module " + helper.GetUniqueName();
                enaInput.SetName(enaName);
                ena.ConnectToInput(enaInput);
                helper.Mod.AddExternalIO(enaInput);

                //Connect internal enable wire to dummy component so it's
                //not disconnected later because it isn't used
                var internalEna = (GraphFIR.IO.Output)helper.Mod.GetIO(enaName);
                var internalEnaDummy = new GraphFIR.DummySink(internalEna);
                helper.AddNodeToModule(internalEnaDummy);

                //Set signal that enables this scope as things like memory
                //ports need it
                helper.EnterEnabledScope(internalEna);

                //Make io from parent module visible to child module
                //and connect all the io to the child module
                parentHelper.Mod.CopyInternalAsExternalIO(helper.Mod);

                //Fill out module
                VisitStatement(helper, body);

                //Default things to do when a module is finished
                CleanupModule(helper);

                //If internal io was used then connect its external io
                //to parent modules corresponding io
                helper.Mod.ExternalConnectUsedIO(parentHelper.Mod);

                //If external io wasn't used internally then disconnect
                //external io. Removing io from a module isn't currently
                //possible. In order to avoid visualing all this unused io,
                //unused is hidden in the visualization but that can only work
                //if it's not connected to anything.
                helper.Mod.DisconnectUnusedIO();

                cond.AddConditionalModule((GraphFIR.IO.Input)internalEnaDummy.InIO, helper.Mod);

                helper.ExitEnabledScope();
            }

            GraphFIR.IO.Output enableCond = (GraphFIR.IO.Output)VisitExp(parentHelper, conditional.Pred, GraphFIR.IO.IOGender.Male);

            if (conditional.HasIf())
            {
                AddCondModule(enableCond, conditional.WhenTrue);
            }
            if (conditional.HasElse())
            {
                GraphFIR.FIRNot notEnableComponent = new GraphFIR.FIRNot(enableCond, new FIRRTL.UIntType(1), null);
                parentHelper.AddNodeToModule(notEnableComponent);
                GraphFIR.IO.Output elseEnableCond = notEnableComponent.Result;

                AddCondModule(elseEnableCond, conditional.Alt);
            }

            parentHelper.Mod.AddConditional(cond);
        }

        private static GraphFIR.IO.FIRIO VisitExp(VisitHelper helper, FIRRTL.Expression exp, GraphFIR.IO.IOGender gender)
        {
            if (exp is FIRRTL.RefLikeExpression)
            {
                return (GraphFIR.IO.FIRIO)VisitRef(helper, exp, helper.Mod, gender);
            }

            if (exp is FIRRTL.Literal lit)
            {
                GraphFIR.ConstValue value = new GraphFIR.ConstValue(helper.GetUniqueName(), lit);

                helper.AddNodeToModule(value);
                return value.Result;
            }
            else if (exp is FIRRTL.DoPrim prim)
            {
                var args = prim.Args.Select(x => VisitExp(helper, x, GraphFIR.IO.IOGender.Male)).Cast<GraphFIR.IO.Output>().ToArray();
                GraphFIR.FIRRTLPrimOP nodePrim;
                if (prim.Op is FIRRTL.Add)
                {
                    nodePrim = new GraphFIR.FIRAdd(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Sub)
                {
                    nodePrim = new GraphFIR.FIRSub(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Mul)
                {
                    nodePrim = new GraphFIR.FIRMul(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Div)
                {
                    nodePrim = new GraphFIR.FIRDiv(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Rem)
                {
                    nodePrim = new GraphFIR.FIRRem(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Dshl)
                {
                    nodePrim = new GraphFIR.FIRDshl(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Dshr)
                {
                    nodePrim = new GraphFIR.FIRDshr(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Cat)
                {
                    nodePrim = new GraphFIR.FIRCat(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Eq)
                {
                    nodePrim = new GraphFIR.FIREq(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Neq)
                {
                    nodePrim = new GraphFIR.FIRNeq(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Geq)
                {
                    nodePrim = new GraphFIR.FIRGeq(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Leq)
                {
                    nodePrim = new GraphFIR.FIRLeq(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Gt)
                {
                    nodePrim = new GraphFIR.FIRGt(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Lt)
                {
                    nodePrim = new GraphFIR.FIRLt(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.And)
                {
                    nodePrim = new GraphFIR.FIRAnd(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Or)
                {
                    nodePrim = new GraphFIR.FIROr(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Xor)
                {
                    nodePrim = new GraphFIR.FIRXor(args[0], args[1], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Head)
                {
                    nodePrim = new GraphFIR.Head(args[0], prim.Type, (int)prim.Consts[0], prim);
                }
                else if (prim.Op is FIRRTL.Tail)
                {
                    nodePrim = new GraphFIR.Tail(args[0], prim.Type, (int)prim.Consts[0], prim);
                }
                else if (prim.Op is FIRRTL.Bits)
                {
                    nodePrim = new GraphFIR.BitExtract(args[0], prim.Type, (int)prim.Consts[1], (int)prim.Consts[0], prim);
                }
                else if (prim.Op is FIRRTL.Pad)
                {
                    nodePrim = new GraphFIR.Pad(args[0], prim.Type, (int)prim.Consts[0], prim);
                }
                else if (prim.Op is FIRRTL.AsUInt)
                {
                    nodePrim = new GraphFIR.FIRAsUInt(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.AsSInt)
                {
                    nodePrim = new GraphFIR.FIRAsSInt(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.AsClock)
                {
                    nodePrim = new GraphFIR.FIRAsClock(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Cvt)
                {
                    nodePrim = new GraphFIR.FIRCvt(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Neg)
                {
                    nodePrim = new GraphFIR.FIRNeg(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Not)
                {
                    nodePrim = new GraphFIR.FIRNot(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Andr)
                {
                    nodePrim = new GraphFIR.FIRAndr(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Orr)
                {
                    nodePrim = new GraphFIR.FIROrr(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Xorr)
                {
                    nodePrim = new GraphFIR.FIRXorr(args[0], prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Shl)
                {
                    var constLit = new FIRRTL.UIntLiteral(prim.Consts[0], 0);
                    var constOutput = (GraphFIR.IO.Output)VisitExp(helper, constLit, GraphFIR.IO.IOGender.Male);
                    nodePrim = new GraphFIR.FIRShl(args[0], constOutput, prim.Type, prim);
                }
                else if (prim.Op is FIRRTL.Shr)
                {
                    var constLit = new FIRRTL.UIntLiteral(prim.Consts[0], 0);
                    var constOutput = (GraphFIR.IO.Output)VisitExp(helper, constLit, GraphFIR.IO.IOGender.Male);
                    nodePrim = new GraphFIR.FIRShr(args[0], constOutput, prim.Type, prim);
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
                var cond = (GraphFIR.IO.Output)VisitExp(helper, mux.Cond, GraphFIR.IO.IOGender.Male);
                var ifTrue = VisitExp(helper, mux.TrueValue, GraphFIR.IO.IOGender.Male);
                var ifFalse = VisitExp(helper, mux.FalseValue, GraphFIR.IO.IOGender.Male);

                GraphFIR.Mux node = new GraphFIR.Mux(new List<GraphFIR.IO.FIRIO>() { ifTrue, ifFalse }, cond, mux);

                helper.AddNodeToModule(node);
                return node.Result;
            }
            else if (exp is FIRRTL.ValidIf validIf)
            {
                var cond = (GraphFIR.IO.Output)VisitExp(helper, validIf.Cond, GraphFIR.IO.IOGender.Male);
                var ifValid = VisitExp(helper, validIf.Value, GraphFIR.IO.IOGender.Male);

                GraphFIR.Mux node = new GraphFIR.Mux(new List<GraphFIR.IO.FIRIO>() { ifValid }, cond, validIf);

                helper.AddNodeToModule(node);
                return node.Result;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static GraphFIR.IO.IContainerIO VisitRef(VisitHelper helper, FIRRTL.Expression exp, GraphFIR.IO.IContainerIO currContainer, GraphFIR.IO.IOGender gender)
        {
            GraphFIR.IO.IContainerIO refContainer;
            if (exp is FIRRTL.Reference reference)
            {
                refContainer = currContainer.GetIO(reference.Name);
            }
            else if (exp is FIRRTL.SubField subField)
            {
                refContainer = VisitExp(helper, subField.Expr, gender).GetIO(subField.Name);
            }
            else if (exp is FIRRTL.SubIndex subIndex)
            {
                var subVec = VisitExp(helper, subIndex.Expr, gender);
                var vec = (GraphFIR.IO.Vector)GetIOGender(helper, subVec, gender);

                refContainer = vec.GetIndex(subIndex.Value);
            }
            else if (exp is FIRRTL.SubAccess subAccess)
            {
                var subVec = VisitExp(helper, subAccess.Expr, gender);
                var vec = (GraphFIR.IO.Vector)GetIOGender(helper, subVec, gender);
                var index = (GraphFIR.IO.Output)VisitExp(helper, subAccess.Index, GraphFIR.IO.IOGender.Male);

                refContainer = vec.MakeWriteAccess(index, gender);
            }
            else
            {
                throw new NotImplementedException();
            }

            //Never return bigender io. Only this method should have to deal
            //with that mess so the rest of the code doesn't have to.
            //Dealing with it is ugly which is why i want to contain it.
            if (refContainer is GraphFIR.IO.FIRIO firIO && refContainer is not GraphFIR.IO.IPreserveDuplex)
            {
                return GetIOGender(helper, firIO, gender);
            }
            return refContainer;
        }

        private static GraphFIR.IO.FIRIO GetIOGender(VisitHelper helper, GraphFIR.IO.FIRIO io, GraphFIR.IO.IOGender gender)
        {
            if (io is GraphFIR.IO.Input input && gender == GraphFIR.IO.IOGender.Male)
            {
                if (helper.Mod.TryGetDuplexOutputWire(input, out GraphFIR.IO.DuplexIO wireDuplex))
                {
                    io = wireDuplex;
                }
                else
                {
                    //Full name of io may collide with name of some other io, so
                    //an unique string is added
                    string fullNameUnique = input.GetFullName() + helper.GetUniqueName();
                    GraphFIR.Wire wire = new GraphFIR.Wire(fullNameUnique, input, null);
                    helper.Mod.AddWire(wire);
                    helper.Mod.AddDuplexOuputWire(input, wire.GetAsDuplex());

                    io = wire.GetAsDuplex();
                }
            }

            return io.GetAsGender(gender);
        }
    }
}
