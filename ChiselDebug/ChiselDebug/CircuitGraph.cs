using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug
{
    public class CircuitGraph
    {
        public readonly string Name;
        public readonly List<GraphFIR.Module> Modules = new List<GraphFIR.Module>();

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
                nodeOut.output.SetName(name);
                nameToOutput.Add(name, nodeOut.output);
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
                throw new NotImplementedException();
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
                GraphFIR.ConstValue value = new GraphFIR.ConstValue(lit);

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
                else
                {
                    throw new NotImplementedException();
                }

                outToNode.Add(nodePrim.Result, nodePrim);
                module.AddNode(nodePrim);
                return (nodePrim, nodePrim.Result);
            }
            else if (exp is FIRRTL.Reference reference)
            {
                GraphFIR.Output output = nameToOutput[reference.Name];
                GraphFIR.FIRRTLNode node = outToNode[output];
                return (node, output);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
