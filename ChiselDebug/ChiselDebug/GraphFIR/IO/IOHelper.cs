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
                                if (input.GetModResideIn() == mod || containedCondMods.Contains(input.GetModResideIn()))
                                {
                                    continue;
                                }

                                Output condition = input.GetConnectionCondition(output);
                                if (condition != null)
                                {
                                    Input flipped;
                                    if (intDstSinkToModPass.TryGetValue(input, out flipped))
                                    {
                                        output.DisconnectInput(input);
                                        output.ConnectToInput(flipped, false, false, condition);
                                    }
                                    else if (intSrcSourceToModPass.TryGetValue(output, out flipped))
                                    {
                                        Output extOutput = (Output)flipped.GetPaired();
                                        input.ReplaceConnection(output, extOutput, mod.EnableCon);
                                    }
                                    else
                                    {
                                        flipped = (Input)output.Flip(mod);
                                        mod.AddAnonymousInternalIO(flipped);
                                        Output extOutput = (Output)flipped.GetPaired();

                                        input.ReplaceConnection(output, extOutput, mod.EnableCon);
                                        //input.ReplaceConnection(output, extOutput, condition);
                                        output.ConnectToInput(flipped, false, false, condition);

                                        intDstSinkToModPass.Add(input, flipped);
                                        intSrcSourceToModPass.Add(output, flipped);
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
                                    Input flipped;
                                    if (extSrcSourceToModPass.TryGetValue(cons.From, out flipped))
                                    {
                                        Output intOutput = (Output)flipped.GetPaired();
                                        input.ReplaceConnection(cons.From, intOutput, cons.Condition);
                                    }
                                    else if (extDstSinkToModPass.TryGetValue(input, out flipped))
                                    {
                                        cons.From.DisconnectInput(input);
                                        cons.From.ConnectToInput(flipped, false, false, cons.Condition);
                                    }
                                    else
                                    {
                                        flipped = (Input)cons.From.Flip(mod);
                                        mod.AddAnonymousExternalIO(flipped);
                                        Output intOutput = (Output)flipped.GetPaired();

                                        input.ReplaceConnection(cons.From, intOutput, cons.Condition);
                                        cons.From.ConnectToInput(flipped, false, false, cons.Condition);

                                        extDstSinkToModPass.Add(input, flipped);
                                        extSrcSourceToModPass.Add(cons.From, flipped);
                                    }
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