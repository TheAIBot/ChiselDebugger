using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ChiselDebug
{
    public class CircuitGraph
    {
        public readonly string Name;
        public readonly List<GraphFIR.Module> Modules = new List<GraphFIR.Module>();

        private static long UniqueNumber;

        private static string GetUniqueName()
        {
            return $"~{Interlocked.Increment(ref UniqueNumber)}";
        }

        public CircuitGraph(string name, List<GraphFIR.Module> modules)
        {
            this.Name = name;
            this.Modules = modules;
        }

        public static CircuitGraph VisitCircuit(FIRRTL.Circuit circuit)
        {
            List<GraphFIR.Module> modules = new List<GraphFIR.Module>();
            foreach (var moduleDef in circuit.Modules)
            {
                modules.Add(VisitModule(moduleDef));
            }

            return new CircuitGraph(circuit.Main, modules);
        }

        public static GraphFIR.Module VisitModule(FIRRTL.DefModule moduleDef)
        {
            if (moduleDef is FIRRTL.Module mod)
            {
                GraphFIR.Module module = new GraphFIR.Module(mod.Name);
                foreach (var port in mod.Ports)
                {
                    VisitPort(module, port);
                }

                var outToNode = new Dictionary<GraphFIR.Output, GraphFIR.FIRRTLNode>();
                var nameToOutput = new Dictionary<string, GraphFIR.Output>();
                foreach (var output in module.InternalOutputs)
                {
                    outToNode.Add(output, module);
                    nameToOutput.Add(output.Name, output);
                }

                var nameToInput = new Dictionary<string, GraphFIR.Input>();
                foreach (var input in module.InternalInputs)
                {
                    nameToInput.Add(input.Name, input);
                }

                VisitStatement(outToNode, nameToOutput, nameToInput, module, mod.Body);

                return module;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static void VisitPort(GraphFIR.Module module, FIRRTL.Port port)
        {
            if (port.Type is FIRRTL.AggregateType)
            {
                throw new NotImplementedException();
            }

            if (port.Direction == FIRRTL.Dir.Input)
            {
                module.AddExternalInput(port.Name, port.Type);
            }
            else
            {
                module.AddExternalOutput(port.Name, port.Type);
            }
        }

        public static void VisitStatement(
            Dictionary<GraphFIR.Output, GraphFIR.FIRRTLNode> outToNode, 
            Dictionary<string, GraphFIR.Output> nameToOutput,
            Dictionary<string, GraphFIR.Input> nameToInput,
            GraphFIR.Module module, FIRRTL.Statement statement)
        {
            if (statement is FIRRTL.EmptyStmt)
            {
                return;
            }
            else if (statement is FIRRTL.Block block)
            {
                block.Statements.ForEach(x => VisitStatement(outToNode, nameToOutput, nameToInput, module, x));
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
                var nodeOut = VisitExp(outToNode, nameToOutput, module, connect.Expr);
                string name = ((FIRRTL.Reference)connect.Loc).Name;
                if (nameToOutput.TryGetValue(name, out var maybeRegOut) && 
                    outToNode.TryGetValue(maybeRegOut, out var maybeReg) && 
                    maybeReg is GraphFIR.Register)
                {
                    name = name + "/in";
                }

                nodeOut.output.SetName(name);
                if (!nameToOutput.ContainsKey(name))
                {
                    nameToOutput.Add(name, nodeOut.output);
                }
                var input = nameToInput[name];
                nodeOut.output.ConnectToInput(input);
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
                var clock = VisitExp(outToNode, nameToOutput, module, reg.Clock);
                GraphFIR.Register register;

                //if it has no reset then it also has no init value
                if (reg.Reset is FIRRTL.UIntLiteral res && res.Value == 0)
                {
                    register = new GraphFIR.Register(reg.Name, clock.output, null, null, reg.Type);
                }
                else
                {
                    var reset = VisitExp(outToNode, nameToOutput, module, reg.Reset);
                    var initValue = VisitExp(outToNode, nameToOutput, module, reg.Init);
                    register = new GraphFIR.Register(reg.Name, clock.output, reset.output, initValue.output, reg.Type);
                }

                module.AddNode(register);
                outToNode.Add(register.Result, register);
                nameToInput.Add(register.Name + "/in", register.In);
                nameToOutput.Add(register.Name, register.Result);
            }
            else if (statement is FIRRTL.DefInstance)
            {
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.DefNode node)
            {
                var nodeOut = VisitExp(outToNode, nameToOutput, module, node.Value);
                nodeOut.output.SetName(node.Name);
                nameToOutput.Add(node.Name, nodeOut.output);
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

        private static (GraphFIR.FIRRTLNode node, GraphFIR.Output output) VisitExp(Dictionary<GraphFIR.Output, GraphFIR.FIRRTLNode> outToNode, Dictionary<string, GraphFIR.Output> nameToOutput, GraphFIR.Module module, FIRRTL.Expression exp)
        {
            if (exp is FIRRTL.Literal lit)
            {
                GraphFIR.ConstValue value = new GraphFIR.ConstValue(GetUniqueName(), lit);

                outToNode.Add(value.Result, value);
                module.AddNode(value);
                return (value, value.Result);
            }
            else if (exp is FIRRTL.DoPrim prim)
            {
                var args = prim.Args.Select(x => VisitExp(outToNode, nameToOutput, module, x)).ToArray();
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
                    nodePrim = new GraphFIR.BitExtract(args[0].output, prim.Type, (int)prim.Consts[0], (int)prim.Consts[1]);
                }
                else
                {
                    throw new NotImplementedException();
                }

                outToNode.Add(nodePrim.Result, nodePrim);
                module.AddNode(nodePrim);
                return (nodePrim, nodePrim.Result);
            }
            else if (exp is FIRRTL.Mux mux)
            {
                var cond = VisitExp(outToNode, nameToOutput, module, mux.Cond);
                var ifTrue = VisitExp(outToNode, nameToOutput, module, mux.TrueValue);
                var ifFalse = VisitExp(outToNode, nameToOutput, module, mux.FalseValue);

                GraphFIR.Mux node = new GraphFIR.Mux(new List<FIRRTL.IFIRType>() { ifTrue.output.Type, ifFalse.output.Type }, mux.Type);
                cond.output.ConnectToInput(node.Decider);
                ifTrue.output.ConnectToInput(node.Choises[0]);
                ifFalse.output.ConnectToInput(node.Choises[1]);

                outToNode.Add(node.Result, node);
                module.AddNode(node);
                return (node, node.Result);
            }
            else if (exp is FIRRTL.Reference reference)
            {
                GraphFIR.Output output = nameToOutput[reference.Name];
                GraphFIR.FIRRTLNode node = outToNode[output];
                return (node, output);
            }
            else if (exp is FIRRTL.ValidIf validIf)
            {
                var cond = VisitExp(outToNode, nameToOutput, module, validIf.Cond);
                var ifValid = VisitExp(outToNode, nameToOutput, module, validIf.Value);

                GraphFIR.Mux node = new GraphFIR.Mux(new List<FIRRTL.IFIRType>() { ifValid.output.Type }, validIf.Type);
                cond.output.ConnectToInput(node.Decider);
                ifValid.output.ConnectToInput(node.Choises[0]);

                outToNode.Add(node.Result, node);
                module.AddNode(node);
                return (node, node.Result);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
