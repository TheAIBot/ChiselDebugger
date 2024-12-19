using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ChiselDebug.GraphFIR.Transformations
{
    internal static class BypassCondModBorderConnections
    {
        public static void Transform(Module mainModule)
        {
            foreach (var mod in mainModule.GetAllNestedNodesOfType<Module>())
            {
                MakeAllCondConnectionsGoThroughCondModule(mod);
            }

            foreach (var mod in mainModule.GetAllNestedNodesOfType<Module>())
            {
                if (mod.IsConditional)
                {
                    BypassThroughCondModules(mod);
                }
            }
        }

        private static void MakeAllCondConnectionsGoThroughCondModule(Module mod)
        {
            //Some conditional connections do not originate from a conditional
            //module because both the source and sink didn't reside in a conditional
            //module. Fix this by rerouting the connection so it goes through the
            //conditional module.
            Dictionary<(Source, Source), Source> sourceToCondSource = new Dictionary<(Source, Source), Source>();
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
                        Module condMod = connection.Condition.GetModResideInThrowIfNull();
                        Sink extIn = (Sink)input.Copy(condMod);
                        Sink intIn = (Sink)input.Copy(condMod);
                        condMod.AddAnonymousExternalIO(extIn);
                        condMod.AddAnonymousInternalIO(intIn);
                        Source extOut = intIn.GetPairedThrowIfNull();
                        Source intOut = extIn.GetPairedThrowIfNull();

                        connection.From.ConnectToInput(extIn);
                        intOut.ConnectToInput(intIn);
                        input.ReplaceConnection(connection, extOut);

                        sourceToCondSource.Add((connection.From, connection.Condition), extOut);
                    }
                }
            }
        }

        private static void BypassThroughCondModules(Module mod)
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

            Dictionary<Connection, Source> bypasses = new Dictionary<Connection, Source>();
            foreach (var node in mod.GetAllNodes())
            {
                foreach (var io in node.GetIO().ToArray())
                {
                    if (TryIsIOPassiveAggWithSingleAggEndpoint(io, out var endpointConnections))
                    {
                        BypassAggregateConnections(mod, containedCondMods, (AggregateIO)io, endpointConnections);
                    }

                    foreach (var scalar in io.Flatten())
                    {
                        if (scalar is Source output)
                        {
                            foreach (var input in output.GetConnectedInputs().ToArray())
                            {
                                foreach (var connection in input.GetConnections())
                                {
                                    if (connection.From != output)
                                    {
                                        continue;
                                    }
                                    if (input.GetModResideIn() == mod || containedCondMods.Contains(input.GetModResideInThrowIfNull()))
                                    {
                                        continue;
                                    }

                                    if (connection.Condition != null)
                                    {
                                        if (bypasses.TryGetValue(connection, out var bypassOutput))
                                        {
                                            input.ReplaceConnection(connection, bypassOutput);
                                            continue;
                                        }

                                        Sink flipped = (Sink)connection.From.Flip(mod);
                                        mod.AddAnonymousInternalIO(flipped);
                                        Source extOutput = flipped.GetPairedThrowIfNull();

                                        connection.From.ConnectToInput(flipped, false, false, connection.Condition);
                                        input.ReplaceConnection(connection, extOutput);
                                        bypasses.Add(connection, extOutput);
                                    }
                                }
                            }
                        }
                        else if (scalar is Sink input)
                        {
                            foreach (var connection in input.GetConnections())
                            {
                                if (connection.From.GetModResideIn() == mod || containedCondMods.Contains(connection.From.GetModResideInThrowIfNull()))
                                {
                                    continue;
                                }

                                if (connection.From.GetModResideIn() != mod)
                                {
                                    if (bypasses.TryGetValue(connection, out var bypassOutput))
                                    {
                                        input.ReplaceConnection(connection, bypassOutput);
                                        continue;
                                    }

                                    Sink flipped = (Sink)connection.From.Flip(mod);
                                    mod.AddAnonymousExternalIO(flipped);
                                    Source intOutput = flipped.GetPairedThrowIfNull();

                                    connection.From.ConnectToInput(flipped, false, false, connection.Condition);
                                    input.ReplaceConnection(connection, intOutput);
                                    bypasses.Add(connection, intOutput);
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

        private static void BypassAggregateConnections(Module mod, HashSet<Module> containedCondMods, AggregateIO io, AggregateConnection[] endpointConnections)
        {
            foreach (var endpointConnection in endpointConnections)
            {
                var ioConnections = GetScalarConnections(io, endpointConnection.To).ToList();
                if (io.IsPassiveOfType<Source>())
                {
                    bool needToBypass = true;
                    foreach (var ioConnection in ioConnections)
                    {
                        if (ioConnection.input.GetModResideIn() == mod || containedCondMods.Contains(ioConnection.input.GetModResideInThrowIfNull()))
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
                    AggregateIO? extAggIO = flipped.Flatten().First().GetPairedThrowIfNull().ParentIO;
                    if (extAggIO == null)
                    {
                        throw new InvalidOperationException("Flipped scalar IO has no parent aggregate IO which it should.");
                    }

                    io.ConnectToInput(flipped, false, false, endpointConnection.Condition);
                    endpointConnection.To.ReplaceConnection(io, extAggIO, endpointConnection.Condition);
                }
                else if (io.IsPassiveOfType<Sink>())
                {
                    bool needToBypass = true;
                    foreach (var ioConnection in ioConnections)
                    {
                        if (ioConnection.output.GetModResideIn() == mod || containedCondMods.Contains(ioConnection.output.GetModResideInThrowIfNull()))
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
                    AggregateIO? extAggIO = flipped.Flatten().First().GetPairedThrowIfNull().ParentIO;
                    if (extAggIO == null)
                    {
                        throw new InvalidOperationException("Flipped scalar IO has no parent aggregate IO which it should.");
                    }

                    endpointConnection.To.ConnectToInput(extAggIO, false, false, endpointConnection.Condition);
                    io.ReplaceConnection(endpointConnection.To, (AggregateIO)flipped, endpointConnection.Condition);
                }
                else
                {
                    throw new Exception($"Unknown io: {io.GetType()}");
                }
            }
        }

        private static bool TryIsIOPassiveAggWithSingleAggEndpoint(FIRIO io, [NotNullWhen(true)] out AggregateConnection[]? endpointConnections)
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

        private static IEnumerable<(Source output, Sink input)> GetScalarConnections(AggregateIO a, AggregateIO b)
        {
            return a.Flatten()
                    .Zip(b.Flatten())
                    .Select(x => IOHelper.OrderIO(x.First, x.Second));
        }
    }
}
