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
        public readonly Dictionary<string, FIRRTL.DefModule> ModuleRoots;

        private readonly Stack<GraphFIR.FIRRTLNode> ScopeEnabledConditions = new Stack<GraphFIR.FIRRTLNode>();
        public GraphFIR.FIRRTLNode ScopeEnabledCond 
        { 
            get
            {
                if (ScopeEnabledConditions.Count == 0)
                {
                    GraphFIR.ConstValue constEnabled = new GraphFIR.ConstValue(GetUniqueName(), new FIRRTL.UIntLiteral(1, 1));
                    AddNodeToModule(constEnabled);

                    ScopeEnabledConditions.Push(constEnabled);
                }

                return ScopeEnabledConditions.Peek();
            }
        }

        public VisitHelper(GraphFIR.Module mod) : this(mod, new Dictionary<string, FIRRTL.DefModule>())
        { }

        public VisitHelper(GraphFIR.Module mod, Dictionary<string, FIRRTL.DefModule> roots)
        {
            this.Mod = mod;
            this.ModuleRoots = roots;
        }

        public VisitHelper ForNewModule(string moduleName)
        {
            return new VisitHelper(new GraphFIR.Module(moduleName), ModuleRoots);
        }

        public void AddNodeToModule(GraphFIR.FIRRTLNode node)
        {
            Mod.AddNode(node);
        }

        public void EnterEnabledScope(GraphFIR.FIRRTLNode enableCond)
        {
            ScopeEnabledConditions.Push(enableCond);
        }

        public void ExitEnabledScope()
        {
            ScopeEnabledConditions.Pop();
        }

        private static long UniqueNumber = 0;

        internal string GetUniqueName()
        {
            return $"~{Interlocked.Increment(ref UniqueNumber)}";
        }
    }

    public static class CircuitToGraph
    {
        public static CircuitGraph GetAsGraph(FIRRTL.Circuit circuit)
        {
            VisitHelper helper = new VisitHelper(null);
            foreach (var moduleDef in circuit.Modules)
            {
                helper.ModuleRoots.Add(moduleDef.Name, moduleDef);
            }

            List<GraphFIR.Module> modules = new List<GraphFIR.Module>();
            foreach (var moduleDef in circuit.Modules)
            {
                modules.Add(VisitModule(helper, moduleDef));
            }

            FIRRTL.DefModule mainModDef = circuit.Modules.Single(x => x.Name == circuit.Main);
            GraphFIR.Module mainModule = VisitModule(helper, mainModDef);
            return new CircuitGraph(circuit.Main, mainModule);
        }

        private static GraphFIR.Module VisitModule(VisitHelper parentHelper, FIRRTL.DefModule moduleDef)
        {
            if (moduleDef is FIRRTL.Module mod)
            {
                VisitHelper helper = parentHelper.ForNewModule(mod.Name);
                foreach (var port in mod.Ports)
                {
                    VisitPort(helper, port);
                }

                VisitStatement(helper, mod.Body);

                return helper.Mod;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void VisitPort(VisitHelper helper, FIRRTL.Port port)
        {
            foreach (var io in VisitType(helper, port.Direction, port.Name, port.Type))
            {
                helper.Mod.AddExternalIO(io);
            }
        }

        private static List<GraphFIR.IO.FIRIO> VisitType(VisitHelper helper, FIRRTL.Dir direction, string name, FIRRTL.IFIRType type)
        {
            List<GraphFIR.IO.FIRIO> io = new List<GraphFIR.IO.FIRIO>();
            if (type is FIRRTL.BundleType bundle)
            {
                io.Add(VisitBundle(helper, direction, name, bundle));
            }
            else if (type is FIRRTL.AggregateType)
            {
                throw new NotImplementedException();
            }

            if (direction == FIRRTL.Dir.Input)
            {
                io.Add(new GraphFIR.IO.Input(null, name, type));

                //VCD may keep track of the previous clock cycle value even if it doesn't use it.
                //To keep in line with supporting everything in the VCD, an input representing
                //the previous clock cycle value is added so there isn't an issue when loading
                //a CircuitState from VCD.
                if (type is FIRRTL.ClockType)
                {
                    io.Add(new GraphFIR.IO.Input(null, name + "/prev", type));
                }
            }
            else
            {
                io.Add(new GraphFIR.IO.Output(null, name, type));
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

            return new GraphFIR.IO.IOBundle(bundleName, io);
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
            else if (statement is FIRRTL.Conditionally)
            {
                throw new NotImplementedException();
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
                GraphFIR.IO.FIRIO from = VisitExp(helper, connect.Expr);
                GraphFIR.IO.FIRIO to = (GraphFIR.IO.FIRIO)VisitRef(helper, connect.Loc, helper.Mod);

                from.GetOutput().ConnectToInput(to.GetInput());
            }
            else if (statement is FIRRTL.PartialConnect)
            {
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.IsInvalid)
            {
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.CDefMemory cmem)
            {
                GraphFIR.IO.FIRIO inputType = VisitType(helper, FIRRTL.Dir.Input, string.Empty, cmem.Type).Single();
                var memory = new GraphFIR.Memory(cmem.Name, inputType, cmem.Size, 0, 0, cmem.Ruw);

                helper.Mod.AddMemory(memory);
            }
            else if (statement is FIRRTL.CDefMPort memPort)
            {
                var memory = helper.Mod.GetMemory(memPort.Mem);
                GraphFIR.MemPort port = memPort.Direction switch
                {
                    FIRRTL.MPortDir.MInfer => throw new NotImplementedException(),
                    FIRRTL.MPortDir.MRead => memory.AddReadPort(memPort.Name),
                    FIRRTL.MPortDir.MWrite => memory.AddWritePort(memPort.Name),
                    FIRRTL.MPortDir.MReadWrite => memory.AddReadWritePort(memPort.Name),
                    var error => throw new Exception($"Unknown memory port type. Type: {error}")
                };

                VisitExp(helper, memPort.Exps[0]).ConnectToInput(port.Address);
                VisitExp(helper, memPort.Exps[1]).ConnectToInput(port.Clock);
                helper.ScopeEnabledCond.GetOutputs().Single().ConnectToInput(port.Enabled);

                helper.Mod.AddMemoryPort(memory, port);
            }
            else if (statement is FIRRTL.DefWire)
            {
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.DefRegister reg)
            {
                GraphFIR.IO.Output clock = (GraphFIR.IO.Output)VisitExp(helper, reg.Clock);
                GraphFIR.IO.Output reset = null;
                GraphFIR.IO.Output initValue = null;

                if (reg.HasResetAndInit())
                {
                    reset = (GraphFIR.IO.Output)VisitExp(helper, reg.Reset);
                    initValue = (GraphFIR.IO.Output)VisitExp(helper, reg.Init);
                }

                GraphFIR.Register register = new GraphFIR.Register(reg.Name, clock, reset, initValue, reg.Type);
                helper.Mod.AddRegister(register);
            }
            else if (statement is FIRRTL.DefInstance instance)
            {
                GraphFIR.Module mod = VisitModule(helper, helper.ModuleRoots[instance.Module]);
                helper.Mod.AddModule(mod, instance.Name);
            }
            else if (statement is FIRRTL.DefNode node)
            {
                var nodeOut = VisitExp(helper, node.Value);

                if (node.Value is not FIRRTL.RefLikeExpression)
                {
                    nodeOut.SetName(node.Name);
                }

                helper.Mod.AddIORename(node.Name, nodeOut);
            }
            else if (statement is FIRRTL.DefMemory mem)
            {
                GraphFIR.IO.FIRIO inputType = VisitType(helper, FIRRTL.Dir.Input, string.Empty, mem.Type).Single();
                var memory = new GraphFIR.Memory(mem.Name, inputType, mem.Depth, mem.ReadLatency, mem.WriteLatency, mem.Ruw);

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

        private static GraphFIR.IO.FIRIO VisitExp(VisitHelper helper, FIRRTL.Expression exp)
        {
            if (exp is FIRRTL.RefLikeExpression)
            {
                return (GraphFIR.IO.FIRIO)VisitRef(helper, exp, helper.Mod);
            }

            if (exp is FIRRTL.Literal lit)
            {
                GraphFIR.ConstValue value = new GraphFIR.ConstValue(helper.GetUniqueName(), lit);

                helper.AddNodeToModule(value);
                return value.Result;
            }
            else if (exp is FIRRTL.DoPrim prim)
            {
                var args = prim.Args.Select(x => VisitExp(helper, x).GetOutput()).Cast<GraphFIR.IO.Output>().ToArray();
                GraphFIR.FIRRTLPrimOP nodePrim;
                if (prim.Op is FIRRTL.Add)
                {
                    nodePrim = new GraphFIR.FIRAdd(args[0], args[1], prim.Type);
                }
                else if (prim.Op is FIRRTL.Sub)
                {
                    nodePrim = new GraphFIR.FIRSub(args[0], args[1], prim.Type);
                }
                else if (prim.Op is FIRRTL.Mul)
                {
                    nodePrim = new GraphFIR.FIRMul(args[0], args[1], prim.Type);
                }
                else if (prim.Op is FIRRTL.Div)
                {
                    nodePrim = new GraphFIR.FIRDiv(args[0], args[1], prim.Type);
                }
                else if (prim.Op is FIRRTL.Eq)
                {
                    nodePrim = new GraphFIR.FIREq(args[0], args[1], prim.Type);
                }
                else if (prim.Op is FIRRTL.Neq)
                {
                    nodePrim = new GraphFIR.FIRNeq(args[0], args[1], prim.Type);
                }
                else if (prim.Op is FIRRTL.Geq)
                {
                    nodePrim = new GraphFIR.FIRGeq(args[0], args[1], prim.Type);
                }
                else if (prim.Op is FIRRTL.Leq)
                {
                    nodePrim = new GraphFIR.FIRLeq(args[0], args[1], prim.Type);
                }
                else if (prim.Op is FIRRTL.Gt)
                {
                    nodePrim = new GraphFIR.FIRGt(args[0], args[1], prim.Type);
                }
                else if (prim.Op is FIRRTL.Lt)
                {
                    nodePrim = new GraphFIR.FIRLt(args[0], args[1], prim.Type);
                }
                else if (prim.Op is FIRRTL.And)
                {
                    nodePrim = new GraphFIR.FIRAnd(args[0], args[1], prim.Type);
                }
                else if (prim.Op is FIRRTL.Or)
                {
                    nodePrim = new GraphFIR.FIROr(args[0], args[1], prim.Type);
                }
                else if (prim.Op is FIRRTL.Xor)
                {
                    nodePrim = new GraphFIR.FIRXor(args[0], args[1], prim.Type);
                }
                else if (prim.Op is FIRRTL.Head)
                {
                    nodePrim = new GraphFIR.Head(args[0], prim.Type, (int)prim.Consts[0]);
                }
                else if (prim.Op is FIRRTL.Tail)
                {
                    nodePrim = new GraphFIR.Tail(args[0], prim.Type, (int)prim.Consts[0]);
                }
                else if (prim.Op is FIRRTL.Bits)
                {
                    nodePrim = new GraphFIR.BitExtract(args[0], prim.Type, (int)prim.Consts[1], (int)prim.Consts[0]);
                }
                else if (prim.Op is FIRRTL.Pad)
                {
                    nodePrim = new GraphFIR.Pad(args[0], prim.Type, (int)prim.Consts[0]);
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
                var cond = (GraphFIR.IO.Output)VisitExp(helper, mux.Cond).GetOutput();
                var ifTrue = VisitExp(helper, mux.TrueValue);
                var ifFalse = VisitExp(helper, mux.FalseValue);

                GraphFIR.Mux node = new GraphFIR.Mux(new List<GraphFIR.IO.FIRIO>() { ifTrue, ifFalse }, cond, mux.Type);

                helper.AddNodeToModule(node);
                return node.Result;
            }
            else if (exp is FIRRTL.ValidIf validIf)
            {
                var cond = (GraphFIR.IO.Output)VisitExp(helper, validIf.Cond).GetOutput();
                var ifValid = VisitExp(helper, validIf.Value);

                GraphFIR.Mux node = new GraphFIR.Mux(new List<GraphFIR.IO.FIRIO>() { ifValid }, cond, validIf.Type);

                helper.AddNodeToModule(node);
                return node.Result;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static GraphFIR.IO.IContainerIO VisitRef(VisitHelper helper, FIRRTL.Expression exp, GraphFIR.IO.IContainerIO currContainer)
        {
            if (exp is FIRRTL.Reference reference)
            {
                return currContainer.GetIO(reference.Name);
            }
            else if (exp is FIRRTL.SubField subField)
            {
                return VisitExp(helper, subField.Expr).GetIO(subField.Name);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
