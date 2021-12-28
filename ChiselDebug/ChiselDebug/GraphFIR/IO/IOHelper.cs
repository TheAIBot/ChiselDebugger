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
                    HashSet<ScalarIO> bypassMade = new HashSet<ScalarIO>();
                    if (TryIsIOPassiveAggWithSingleAggEndpoint(io, out var endpointConnections))
                    {
                        foreach (var endpointConnection in endpointConnections)
                        {
                            var ioConnections = GetScalarConnections(io as AggregateIO, endpointConnection.To).ToList();
                            if (io.IsPassiveOfType<Output>())
                            {
                                bool needToBypass = true;
                                foreach (var ioConnection in ioConnections)
                                {
                                    if (ioConnection.input.GetModResideIn() == mod || containedCondMods.Contains(ioConnection.input.GetModResideIn()))
                                    {
                                        needToBypass = false;
                                        break;
                                    }
                                }

                                if (!needToBypass)
                                {
                                    continue;
                                }

                                FIRIO flipped = io.Flip(mod);
                                mod.AddAnonymousInternalIO(flipped);
                                AggregateIO extAggIO = flipped.Flatten().First().GetPaired().ParentIO;

                                io.ConnectToInput(flipped, false, false, endpointConnection.Condition);
                                endpointConnection.To.ReplaceConnection((io as AggregateIO), extAggIO, endpointConnection.Condition);
                            }
                            else if (io.IsPassiveOfType<Input>())
                            {
                                bool needToBypass = true;
                                foreach (var ioConnection in ioConnections)
                                {
                                    if (ioConnection.output.GetModResideIn() == mod || containedCondMods.Contains(ioConnection.output.GetModResideIn()))
                                    {
                                        needToBypass = false;
                                        break;
                                    }

                                    if (ioConnection.output.GetModResideIn() == mod)
                                    {
                                        needToBypass = false;
                                        break;
                                    }
                                }

                                if (!needToBypass)
                                {
                                    continue;
                                }

                                FIRIO flipped = io.Flip(mod);
                                mod.AddAnonymousInternalIO(flipped);
                                AggregateIO extAggIO = flipped.Flatten().First().GetPaired().ParentIO;

                                endpointConnection.To.ConnectToInput(extAggIO, false, false, endpointConnection.Condition);
                                (io as AggregateIO).ReplaceConnection(endpointConnection.To, flipped as AggregateIO, endpointConnection.Condition);
                            }
                            else
                            {
                                throw new Exception($"Unknown io: {io.GetType()}");
                            }
                        }
                    }

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
                            foreach (var connection in input.GetConnections())
                            {
                                if (connection.From.GetModResideIn() == mod || containedCondMods.Contains(connection.From.GetModResideIn()))
                                {
                                    continue;
                                }

                                if (connection.From.GetModResideIn() != mod)
                                {
                                    Input flipped = (Input)connection.From.Flip(mod);
                                    mod.AddAnonymousExternalIO(flipped);
                                    Output intOutput = flipped.GetPaired();

                                    connection.From.ConnectToInput(flipped, false, false, connection.Condition);
                                    input.ReplaceConnection(connection, intOutput);
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

        private static bool TryIsIOPassiveAggWithSingleAggEndpoint(FIRIO io, out AggregateConnection[] endpointConnections)
        {
            if (io is not AggregateIO aggIO)
            {
                endpointConnections = null;
                return false;
            }

            if (!io.IsPassive())
            {
                endpointConnections = null;
                return false;
            }

            endpointConnections = aggIO.GetConnections();
            return true;
        }

        private static IEnumerable<(Output output, Input input)> GetScalarConnections(AggregateIO a, AggregateIO b)
        {
            return a.Flatten()
                    .Zip(b.Flatten())
                    .Select(x => OrderIO(x.First, x.Second));
        }

        public static (Output output, Input input) OrderIO(ScalarIO a, ScalarIO b) => (a, b) switch
        {
            (Output aOut, Input bIn) => (aOut, bIn),
            (Input aIn, Output bOut) => (bOut, aIn),
            _ => throw new Exception()
        };

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