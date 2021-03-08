using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ChiselDebug
{
    internal class VisitHelper
    {
        public readonly GraphFIR.Module Mod;
        public readonly Dictionary<string, FIRRTL.DefModule> ModuleRoots;
        public readonly Dictionary<GraphFIR.Output, GraphFIR.FIRRTLNode> OutToNode = new Dictionary<GraphFIR.Output, GraphFIR.FIRRTLNode>();
        public readonly Dictionary<string, GraphFIR.Output> NameToOutput = new Dictionary<string, GraphFIR.Output>();
        public readonly Dictionary<string, GraphFIR.Input> NameToInput = new Dictionary<string, GraphFIR.Input>();

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

        public void AddNodeToModule(GraphFIR.FIRRTLPrimOP node)
        {
            Mod.AddNode(node);
            OutToNode.Add(node.Result, node);
        }

        public void AddNodeToModule(GraphFIR.FIRRTLNode node)
        {
            Mod.AddNode(node);
            foreach (var output in node.GetOutputs())
            {
                OutToNode.Add(output, node);
            }
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

            return new CircuitGraph(circuit.Main, modules);
        }

        private static GraphFIR.Module VisitModule(VisitHelper parentHelper, FIRRTL.DefModule moduleDef)
        {
            if (moduleDef is FIRRTL.Module mod)
            {
                VisitHelper helper = parentHelper.ForNewModule(mod.Name);
                foreach (var port in mod.Ports)
                {
                    VisitPort(helper.Mod, port);
                }

                foreach (var output in helper.Mod.InternalOutputs)
                {
                    helper.OutToNode.Add(output, helper.Mod);
                    helper.NameToOutput.Add(output.Name, output);
                }

                foreach (var input in helper.Mod.InternalInputs)
                {
                    helper.NameToInput.Add(input.Name, input);
                }

                VisitStatement(helper, mod.Body);

                helper.Mod.FinishModuleSetup();

                return helper.Mod;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void VisitPort(GraphFIR.Module module, FIRRTL.Port port)
        {
            if (port.Type is FIRRTL.AggregateType)
            {
                throw new NotImplementedException();
            }

            if (port.Direction == FIRRTL.Dir.Input)
            {
                module.AddExternalInput(port.Name, port.Type);

                //VCD may keep track of the previous clock cycle value even if it doesn't use it.
                //To keep in line with supporting everything in the VCD, an input representing
                //the previous clock cycle value is added so there isn't an issue when loading
                //a CircuitState from VCD.
                if (port.Type is FIRRTL.ClockType)
                {
                    module.AddExternalInput(port.Name + "/prev", port.Type);
                }
            }
            else
            {
                module.AddExternalOutput(port.Name, port.Type);
            }
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
                GraphFIR.Output fromOutput = VisitExp(helper, connect.Expr).output;

                //Yes it currently only works with references
                string toName = ((FIRRTL.Reference)connect.Loc).Name;

                //The name for a register input is special because /in
                //is added to the name in the vcd file. If name is a register
                //then set name to register input name.
                if (helper.NameToOutput.TryGetValue(toName, out var maybeRegOut) &&
                    helper.OutToNode.TryGetValue(maybeRegOut, out var maybeReg) &&
                    maybeReg is GraphFIR.Register)
                {
                    toName = toName + "/in";
                    helper.Mod.AddOutputRename(toName, fromOutput);
                }

                var toInput = helper.NameToInput[toName];
                fromOutput.ConnectToInput(toInput);
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
                var clock = VisitExp(helper, reg.Clock);
                GraphFIR.Register register;

                //if it has no reset then it also has no init value
                if (reg.Reset is FIRRTL.UIntLiteral res && res.Value == 0)
                {
                    register = new GraphFIR.Register(reg.Name, clock.output, null, null, reg.Type);
                }
                else
                {
                    var reset = VisitExp(helper, reg.Reset);
                    var initValue = VisitExp(helper, reg.Init);
                    register = new GraphFIR.Register(reg.Name, clock.output, reset.output, initValue.output, reg.Type);
                }

                helper.AddNodeToModule(register);
                helper.NameToInput.Add(register.Name + "/in", register.In);
                helper.NameToOutput.Add(register.Name, register.Result);
            }
            else if (statement is FIRRTL.DefInstance)
            {
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.DefNode node)
            {
                var nodeOut = VisitExp(helper, node.Value);

                if (node.Value is FIRRTL.RefLikeExpression)
                {
                    helper.Mod.AddOutputRename(node.Name, nodeOut.output);
                }
                else
                {
                    nodeOut.output.SetName(node.Name);
                }


                helper.NameToOutput.Add(node.Name, nodeOut.output);
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

        private static (GraphFIR.FIRRTLNode node, GraphFIR.Output output) VisitExp(VisitHelper helper, FIRRTL.Expression exp)
        {
            if (exp is FIRRTL.Literal lit)
            {
                GraphFIR.ConstValue value = new GraphFIR.ConstValue(GetUniqueName(), lit);

                helper.AddNodeToModule(value);
                return (value, value.Result);
            }
            else if (exp is FIRRTL.DoPrim prim)
            {
                var args = prim.Args.Select(x => VisitExp(helper, x)).ToArray();
                GraphFIR.FIRRTLPrimOP nodePrim;
                if (prim.Op is FIRRTL.Add)
                {
                    nodePrim = new GraphFIR.FIRAdd(args[0].output, args[1].output, prim.Type);
                }
                else if (prim.Op is FIRRTL.Sub)
                {
                    nodePrim = new GraphFIR.FIRSub(args[0].output, args[1].output, prim.Type);
                }
                else if (prim.Op is FIRRTL.Mul)
                {
                    nodePrim = new GraphFIR.FIRMul(args[0].output, args[1].output, prim.Type);
                }
                else if (prim.Op is FIRRTL.Div)
                {
                    nodePrim = new GraphFIR.FIRDiv(args[0].output, args[1].output, prim.Type);
                }
                else if (prim.Op is FIRRTL.Eq)
                {
                    nodePrim = new GraphFIR.FIREq(args[0].output, args[1].output, prim.Type);
                }
                else if (prim.Op is FIRRTL.Neq)
                {
                    nodePrim = new GraphFIR.FIRNeq(args[0].output, args[1].output, prim.Type);
                }
                else if (prim.Op is FIRRTL.Geq)
                {
                    nodePrim = new GraphFIR.FIRGeq(args[0].output, args[1].output, prim.Type);
                }
                else if (prim.Op is FIRRTL.Leq)
                {
                    nodePrim = new GraphFIR.FIRLeq(args[0].output, args[1].output, prim.Type);
                }
                else if (prim.Op is FIRRTL.Gt)
                {
                    nodePrim = new GraphFIR.FIRGt(args[0].output, args[1].output, prim.Type);
                }
                else if (prim.Op is FIRRTL.Lt)
                {
                    nodePrim = new GraphFIR.FIRLt(args[0].output, args[1].output, prim.Type);
                }
                else if (prim.Op is FIRRTL.And)
                {
                    nodePrim = new GraphFIR.FIRAnd(args[0].output, args[1].output, prim.Type);
                }
                else if (prim.Op is FIRRTL.Or)
                {
                    nodePrim = new GraphFIR.FIROr(args[0].output, args[1].output, prim.Type);
                }
                else if (prim.Op is FIRRTL.Xor)
                {
                    nodePrim = new GraphFIR.FIRXor(args[0].output, args[1].output, prim.Type);
                }
                else if (prim.Op is FIRRTL.Head)
                {
                    nodePrim = new GraphFIR.Head(args[0].output, prim.Type, (int)prim.Consts[0]);
                }
                else if (prim.Op is FIRRTL.Tail)
                {
                    nodePrim = new GraphFIR.Tail(args[0].output, prim.Type, (int)prim.Consts[0]);
                }
                else if (prim.Op is FIRRTL.Bits)
                {
                    nodePrim = new GraphFIR.BitExtract(args[0].output, prim.Type, (int)prim.Consts[1], (int)prim.Consts[0]);
                }
                else if (prim.Op is FIRRTL.Pad)
                {
                    nodePrim = new GraphFIR.Pad(args[0].output, prim.Type, (int)prim.Consts[0]);
                }
                else
                {
                    throw new NotImplementedException();
                }

                nodePrim.Result.SetName(GetUniqueName());
                helper.AddNodeToModule(nodePrim);
                return (nodePrim, nodePrim.Result);
            }
            else if (exp is FIRRTL.Mux mux)
            {
                var cond = VisitExp(helper, mux.Cond);
                var ifTrue = VisitExp(helper, mux.TrueValue);
                var ifFalse = VisitExp(helper, mux.FalseValue);

                GraphFIR.Mux node = new GraphFIR.Mux(new List<FIRRTL.IFIRType>() { ifTrue.output.Type, ifFalse.output.Type }, mux.Type);
                cond.output.ConnectToInput(node.Decider);
                ifTrue.output.ConnectToInput(node.Choises[0]);
                ifFalse.output.ConnectToInput(node.Choises[1]);

                node.Result.SetName(GetUniqueName());
                helper.AddNodeToModule(node);
                return (node, node.Result);
            }
            else if (exp is FIRRTL.Reference reference)
            {
                GraphFIR.Output output = helper.NameToOutput[reference.Name];
                GraphFIR.FIRRTLNode node = helper.OutToNode[output];
                return (node, output);
            }
            else if (exp is FIRRTL.ValidIf validIf)
            {
                var cond = VisitExp(helper, validIf.Cond);
                var ifValid = VisitExp(helper, validIf.Value);

                GraphFIR.Mux node = new GraphFIR.Mux(new List<FIRRTL.IFIRType>() { ifValid.output.Type }, validIf.Type);
                cond.output.ConnectToInput(node.Decider);
                ifValid.output.ConnectToInput(node.Choises[0]);

                node.Result.SetName(GetUniqueName());
                helper.AddNodeToModule(node);
                return (node, node.Result);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

    }
}
