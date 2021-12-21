using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ChiselDebug.GraphFIR.IO
{
    public static class IOHelper
    {
        public static void BypassWire(Input[] bypassFrom, Output[] bypassTo)
        {
            if (bypassFrom.Length != bypassTo.Length)
            {
                throw new Exception($"Can't bypass io when they are not of the same size. From: {bypassFrom.Length}, To: {bypassTo.Length}");
            }

            for (int i = 0; i < bypassFrom.Length; i++)
            {
                Input from = bypassFrom[i];
                Output to = bypassTo[i];

                Connection[] connectFromCons = from.GetConnections();
                Input[]  connectTo = to.GetConnectedInputs().ToArray();
                from.DisconnectAll();

                foreach (var input in connectTo)
                {
                    input.DisconnectAll();
                    foreach (var connection in connectFromCons)
                    {
                        connection.From.ConnectToInput(input, false, false, connection.Condition);
                    }
                }
            }
            
            //After bypassing, the io shouldn't be connected to anything
            Debug.Assert(bypassFrom.All(x => !x.IsConnectedToAnything()));
            Debug.Assert(bypassTo.All(x => !x.IsConnectedToAnything()));
        }

        public static void BypassCondConnectionsThroughCondModules(Module mod)
        {
            //Some conditional connections do not originate from a conditional
            //module because both the source and sink didn't reside in a conditional
            //module. Fix this by rerouting the connection so it goes through the
            //conditional module.
            Dictionary<(Output, Output), Output> sourceToCondSource = new Dictionary<(Output, Output), Output>();
            foreach (var input in mod.GetAllModuleInputs())
            {
                foreach (var connection in input.GetConnections())
                {
                    if (connection.From.GetModResideIn() != mod)
                    {
                        continue;
                    }

                    if (connection.Condition == null)
                    {
                        continue;
                    }

                    if (sourceToCondSource.TryGetValue((connection.From, connection.Condition), out var extCondOut))
                    {
                        input.ReplaceConnection(connection, extCondOut);
                    }
                    else
                    {
                        Module condMod = connection.Condition.GetModResideIn();
                        Input extIn = (Input)input.Copy(condMod);
                        Input intIn = (Input)input.Copy(condMod);
                        condMod.AddAnonymousExternalIO(extIn);
                        condMod.AddAnonymousInternalIO(intIn);
                        Output extOut = intIn.GetPaired();
                        Output intOut = extIn.GetPaired();

                        connection.From.ConnectToInput(extIn);
                        intOut.ConnectToInput(intIn);
                        input.ReplaceConnection(connection, extOut);

                        sourceToCondSource.Add((connection.From, connection.Condition), extOut);
                    }
                }
            }

            BypassThroughCondModules(mod);
        }

        public static void BypassThroughCondModules(Module mod)
        {
            HashSet<Module> containedCondMods = new HashSet<Module>();
            foreach (var node in mod.GetAllNodes())
            {
                if (node is Conditional condNode)
                {
                    foreach (var condMod in condNode.CondMods)
                    {
                        containedCondMods.Add(condMod);
                        BypassThroughCondModules(condMod);
                    }
                }
            }

            if (!mod.IsConditional)
            {
                return;
            }

            List<ScalarIO> scalars = new List<ScalarIO>();
            foreach (var node in mod.GetAllNodes())
            {
                foreach (var io in node.GetIO().ToArray())
                {
                    scalars.Clear();
                    foreach (var scalar in io.Flatten(scalars))
                    {
                        if (scalar is Output output)
                        {
                            foreach (var input in output.GetConnectedInputs().ToArray())
                            {
                                foreach (var connection in input.GetConnections())
                                {
                                    if (connection.From != output)
                                    {
                                        continue;
                                    }
                                    if (input.GetModResideIn() == mod || containedCondMods.Contains(input.GetModResideIn()))
                                    {
                                        continue;
                                    }

                                    if (connection.Condition != null)
                                    {
                                        Input flipped = (Input)connection.From.Flip(mod);
                                        mod.AddAnonymousInternalIO(flipped);
                                        Output extOutput = flipped.GetPaired();

                                        connection.From.ConnectToInput(flipped, false, false, connection.Condition);
                                        input.ReplaceConnection(connection, extOutput);
                                    }
                                }
                            }
                        }
                        else if (scalar is Input input)
                        {
                            foreach (var cons in input.GetConnections())
                            {
                                if (cons.From.GetModResideIn() == mod || containedCondMods.Contains(cons.From.GetModResideIn()))
                                {
                                    continue;
                                }

                                if (cons.From.GetModResideIn() != mod)
                                {
                                    Input flipped = (Input)cons.From.Flip(mod);
                                    mod.AddAnonymousExternalIO(flipped);
                                    Output intOutput = flipped.GetPaired();

                                    cons.From.ConnectToInput(flipped, false, false, cons.Condition);
                                    input.ReplaceConnection(cons, intOutput);
                                }
                            }
                        }
                        else
                        {
                            throw new Exception($"Unknown ScalarIO: {scalar.GetType()}");
                        }
                    }
                }
            }
        }

        public static (Output output, Input input) OrderIO(ScalarIO a, ScalarIO b)
        {
            if (a is Output && b is Input)
            {
                return ((Output)a, (Input)b);
            }
            else if (a is Input && b is Output)
            {
                return ((Output)b, (Input)a);
            }
            else
            {
                throw new Exception();
            }
        }

        private static bool TryIsIOPassiveAggWithSingleAggEndpoint(Module mod, FIRIO io, HashSet<Module> containedCondMods, out List<AggregateIO> endpoints)
        {
            if (io is not AggregateIO)
            {
                endpoints = null;
                return false;
            }

            if (!io.IsPassive())
            {
                endpoints = null;
                return false;
            }

            endpoints = (io as AggregateIO).GetAllOrderlyConnectedEndpoints();
            return true;
        }

        public static bool TryGetParentMemPort(FIRIO io, out MemPort port)
        {
            FIRIO node = io.Flatten().First();
            while (node.IsPartOfAggregateIO)
            {
                if (node is MemPort foundPort1)
                {
                    port = foundPort1;
                    return true;
                }

                node = node.ParentIO;
            }

            if (node is MemPort foundPort2)
            {
                port = foundPort2;
                return true;
            }

            port = null;
            return false;
        }

        public static bool IsIOInMaskableMemPortData(FIRIO io, MemPort port)
        {
            if (!port.HasMask())
            {
                return false;
            }

            FIRIO dataInput = port.GetInput();

            FIRIO node = io;
            while (node.IsPartOfAggregateIO)
            {
                if (node == dataInput)
                {
                    return true;
                }

                node = node.ParentIO;
            }

            if (node == dataInput)
            {
                return true;
            }

            return false;
        }
    }
}