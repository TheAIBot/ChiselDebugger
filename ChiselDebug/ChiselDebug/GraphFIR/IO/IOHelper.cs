using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ChiselDebug.GraphFIR.IO
{
    public static class IOHelper
    {
        public static void BypassIO(Output from, Output to)
        {
            Input[] inputs = from.GetConnectedInputs().ToArray();
            foreach (var input in inputs)
            {
                from.DisconnectInput(input);
                to.ConnectToInput(input);
            }
        }

        public static void BypassIO(FIRIO bypassFrom, FIRIO bypassTo)
        {
            ScalarIO[] bypassFromIO = bypassFrom.Flatten().ToArray();
            ScalarIO[] bypassToIO = bypassTo.Flatten().ToArray();

            if (bypassFromIO.Length != bypassToIO.Length)
            {
                throw new Exception($"Can't bypass io when they are not of the same size. From: {bypassFromIO.Length}, To: {bypassToIO.Length}");
            }

            for (int i = 0; i < bypassFromIO.Length; i++)
            {
                Connection[] connectFromCons;
                Input[] connectTo;

                //They must both either be connected or not.
                //Can't bypass when only input or output is connected.
                if (bypassFromIO[i].IsConnected() ^ bypassToIO[i].IsConnected())
                {
                    throw new Exception("Bypass IO must either both be connected or disconnected.");
                }

                if (bypassFromIO[i] is Input fromIn && bypassToIO[i] is Output toOut)
                {
                    connectFromCons = fromIn.GetConnections();
                    connectTo = toOut.GetConnectedInputs().ToArray();
                    fromIn.DisconnectAll();
                }
                else if (bypassFromIO[i] is Output fromOut && bypassToIO[i] is Input toIn)
                {
                    connectFromCons = toIn.GetConnections();
                    connectTo = fromOut.GetConnectedInputs().ToArray();
                    toIn.DisconnectAll();
                }
                else 
                {
                    throw new Exception($"Bypass must go from Output to Input. IO is {bypassFromIO[i]} and {bypassToIO[i]}");
                }
                
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
            Debug.Assert(bypassFromIO.All(x => !x.IsConnectedToAnything()));
            Debug.Assert(bypassToIO.All(x => !x.IsConnectedToAnything()));
        }

        public static void BypassCondConnectionsThroughCondModules(Module mod)
        {
            HashSet<Module> containedCondMods = new HashSet<Module>();
            foreach (var node in mod.GetAllNodes())
            {
                if (node is Conditional condNode)
                {
                    foreach (var condMod in condNode.CondMods)
                    {
                        containedCondMods.Add(condMod.Mod);
                        BypassCondConnectionsThroughCondModules(condMod.Mod);
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
                                if (input.GetModResideIn() == mod || containedCondMods.Contains(input.GetModResideIn()))
                                {
                                    continue;
                                }

                                Output condition = input.GetConnectionCondition(output);
                                if (condition != null)
                                {
                                    Input flipped = (Input)output.Flip(mod);
                                    mod.AddAnonymousInternalIO(flipped);
                                    Output extOutput = (Output)flipped.GetPaired();


                                    if (mod.IsAnonymousExtIntIO(output))
                                    {
                                        output.ConnectToInput(flipped, false, false, condition);
                                    }
                                    else
                                    {
                                        output.ConnectToInput(flipped);
                                    }

                                    input.ReplaceConnection(output, extOutput, condition);
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
                                    Output intOutput = (Output)flipped.GetPaired();

                                    cons.From.ConnectToInput(flipped, false, false, cons.Condition);
                                    input.ReplaceConnection(cons.From, intOutput, cons.Condition);
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