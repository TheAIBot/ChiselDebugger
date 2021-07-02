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
                        Output extOut = (Output)intIn.GetPaired();
                        Output intOut = (Output)extIn.GetPaired();

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
                        containedCondMods.Add(condMod.Mod);
                        BypassThroughCondModules(condMod.Mod);
                    }
                }
            }

            if (!mod.IsConditional)
            {
                return;
            }



            Dictionary<Input, Input> intDstSinkToModPass = new Dictionary<Input, Input>();
            Dictionary<Output, Input> intSrcSourceToModPass = new Dictionary<Output, Input>();

            Dictionary<Input, Input> extDstSinkToModPass = new Dictionary<Input, Input>();
            Dictionary<Output, Input> extSrcSourceToModPass = new Dictionary<Output, Input>();

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
                                        Input flipped;
                                        if (intDstSinkToModPass.TryGetValue(input, out flipped))
                                        {
                                            input.Disconnect(connection);
                                            output.ConnectToInput(flipped, false, false, connection.Condition);
                                        }
                                        else if (intSrcSourceToModPass.TryGetValue(output, out flipped))
                                        {
                                            Output extOutput = (Output)flipped.GetPaired();
                                            input.ReplaceConnection(connection, extOutput);
                                            //input.ReplaceConnection(output, extOutput, mod.EnableCon);
                                        }
                                        else
                                        {
                                            flipped = (Input)output.Flip(mod);
                                            mod.AddAnonymousInternalIO(flipped);
                                            Output extOutput = (Output)flipped.GetPaired();

                                            input.ReplaceConnection(connection, extOutput);
                                            //input.ReplaceConnection(output, extOutput, condition);
                                            output.ConnectToInput(flipped, false, false, connection.Condition);

                                            intDstSinkToModPass.Add(input, flipped);
                                            intSrcSourceToModPass.Add(output, flipped);
                                        }
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
                                Input flipped;
                                if (extSrcSourceToModPass.TryGetValue(cons.From, out flipped))
                                {
                                    Output intOutput = (Output)flipped.GetPaired();
                                    input.ReplaceConnection(cons, intOutput);
                                }
                                else if (extDstSinkToModPass.TryGetValue(input, out flipped))
                                {
                                    input.Disconnect(cons);
                                    cons.From.ConnectToInput(flipped, false, false, cons.Condition);
                                }
                                else
                                {
                                    flipped = (Input)cons.From.Flip(mod);
                                    mod.AddAnonymousExternalIO(flipped);
                                    Output intOutput = (Output)flipped.GetPaired();

                                    input.ReplaceConnection(cons, intOutput);
                                    cons.From.ConnectToInput(flipped, false, false, cons.Condition);

                                    extDstSinkToModPass.Add(input, flipped);
                                    extSrcSourceToModPass.Add(cons.From, flipped);
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