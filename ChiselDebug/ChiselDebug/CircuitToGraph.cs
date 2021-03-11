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
    }

    public static class CircuitToGraph
    {
        private static long UniqueNumber = 0;

        private static string GetUniqueName()
        {
            return $"~{Interlocked.Increment(ref UniqueNumber)}";
        }

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

        private static List<GraphFIR.FIRIO> VisitType(VisitHelper helper, FIRRTL.Dir direction, string name, FIRRTL.IFIRType type)
        {
            List<GraphFIR.FIRIO> io = new List<GraphFIR.FIRIO>();
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
                io.Add(new GraphFIR.Input(null, name, type));

                //VCD may keep track of the previous clock cycle value even if it doesn't use it.
                //To keep in line with supporting everything in the VCD, an input representing
                //the previous clock cycle value is added so there isn't an issue when loading
                //a CircuitState from VCD.
                if (type is FIRRTL.ClockType)
                {
                    io.Add(new GraphFIR.Input(null, name + "/prev", type));
                }
            }
            else
            {
                io.Add(new GraphFIR.Output(null, name, type));
            }

            return io;
        }

        private static GraphFIR.FIRIO VisitBundle(VisitHelper helper, FIRRTL.Dir direction, string bundleName, FIRRTL.BundleType bundle)
        {
            List<GraphFIR.FIRIO> io = new List<GraphFIR.FIRIO>();
            foreach (var field in bundle.Fields)
            {
                FIRRTL.Dir fieldDir = direction.Flip(field.Flip);
                io.AddRange(VisitType(helper, fieldDir, field.Name, field.Type));
            }

            return new GraphFIR.IOBundle(bundleName, io);
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
                GraphFIR.FIRIO from = VisitExp(helper, connect.Expr);

                GraphFIR.FIRIO to;
                if (connect.Loc is FIRRTL.Reference firRef)
                {
                    //The name for a register input is special because /in
                    //is added to the name in the vcd file. If name is a register
                    //then set name to register input name.
                    if (helper.Mod.GetIO(firRef.Name) is GraphFIR.Output maybeRegOut &&
                        maybeRegOut.Node != null &&
                        maybeRegOut.Node is GraphFIR.Register reg)
                    {
                        to = reg.In;
                    }
                    else
                    {
                        to = (GraphFIR.FIRIO)helper.Mod.GetIO(firRef.Name);
                    }
                }
                else
                {
                    to = (GraphFIR.FIRIO)VisitRef(helper, connect.Loc, helper.Mod);
                }

                from.ConnectToInput(to);
            }
            else if (statement is FIRRTL.PartialConnect)
            {
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.IsInvalid)
            {
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.CDefMemory)
            {
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.CDefMPort)
            {
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.DefWire)
            {
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.DefRegister reg)
            {
                var clock = (GraphFIR.Output)VisitExp(helper, reg.Clock);
                GraphFIR.Register register;

                //if it has no reset then it also has no init value
                if (reg.Reset is FIRRTL.UIntLiteral res && res.Value == 0)
                {
                    register = new GraphFIR.Register(reg.Name, clock, null, null, reg.Type);
                }
                else
                {
                    var reset = (GraphFIR.Output)VisitExp(helper, reg.Reset);
                    var initValue = (GraphFIR.Output)VisitExp(helper, reg.Init);
                    register = new GraphFIR.Register(reg.Name, clock, reset, initValue, reg.Type);
                }

                helper.AddNodeToModule(register);
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
            else if (statement is FIRRTL.DefMemory)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static GraphFIR.FIRIO VisitExp(VisitHelper helper, FIRRTL.Expression exp)
        {
            if (exp is FIRRTL.RefLikeExpression)
            {
                return (GraphFIR.FIRIO)VisitRef(helper, exp, helper.Mod);
            }

            if (exp is FIRRTL.Literal lit)
            {
                GraphFIR.ConstValue value = new GraphFIR.ConstValue(GetUniqueName(), lit);

                helper.AddNodeToModule(value);
                return value.Result;
            }
            else if (exp is FIRRTL.DoPrim prim)
            {
                var args = prim.Args.Select(x => VisitExp(helper, x)).Cast<GraphFIR.Output>().ToArray();
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
                var cond = (GraphFIR.Output)VisitExp(helper, mux.Cond);
                var ifTrue = (GraphFIR.Output)VisitExp(helper, mux.TrueValue);
                var ifFalse = (GraphFIR.Output)VisitExp(helper, mux.FalseValue);

                GraphFIR.Mux node = new GraphFIR.Mux(new List<FIRRTL.IFIRType>() { ifTrue.Type, ifFalse.Type }, mux.Type);
                cond.ConnectToInput(node.Decider);
                ifTrue.ConnectToInput(node.Choises[0]);
                ifFalse.ConnectToInput(node.Choises[1]);

                helper.AddNodeToModule(node);
                return node.Result;
            }
            else if (exp is FIRRTL.ValidIf validIf)
            {
                var cond = (GraphFIR.Output)VisitExp(helper, validIf.Cond);
                var ifValid = (GraphFIR.Output)VisitExp(helper, validIf.Value);

                GraphFIR.Mux node = new GraphFIR.Mux(new List<FIRRTL.IFIRType>() { ifValid.Type }, validIf.Type);
                cond.ConnectToInput(node.Decider);
                ifValid.ConnectToInput(node.Choises[0]);

                helper.AddNodeToModule(node);
                return node.Result;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static GraphFIR.IContainerIO VisitRef(VisitHelper helper, FIRRTL.Expression exp, GraphFIR.IContainerIO currContainer)
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
