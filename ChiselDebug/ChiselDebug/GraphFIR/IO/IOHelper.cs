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
            Input[] inputs = from.Con.GetConnectedInputs().ToArray();
            foreach (var input in inputs)
            {
                from.Con.DisconnectInput(input);
                to.ConnectToInput(input);
            }
        }

        public static void BypassIO(FIRIO bypassFrom, FIRIO bypassTo)
        {
            HashSet<ScalarIO> ignoreFromIO = new HashSet<ScalarIO>(bypassFrom.GetAllIOOfType<VectorAccess>().SelectMany(x => x.Flatten()));
            HashSet<ScalarIO> ignoreToIO = new HashSet<ScalarIO>(bypassTo.GetAllIOOfType<VectorAccess>().SelectMany(x => x.Flatten()));
            foreach (var item in ignoreFromIO)
            {
                item.DisconnectAll();
            }
            foreach (var item in ignoreToIO)
            {
                item.DisconnectAll();
            }

            ScalarIO[] bypassFromIO = bypassFrom.Flatten().Where(x => !ignoreFromIO.Contains(x)).ToArray();
            ScalarIO[] bypassToIO = bypassTo.Flatten().Where(x => !ignoreToIO.Contains(x)).ToArray();

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
                    connectFromCons = fromIn.GetAllConnections();
                    connectTo = toOut.Con.GetConnectedInputs().ToArray();
                    fromIn.DisconnectAll();
                }
                else if (bypassFromIO[i] is Output fromOut && bypassToIO[i] is Input toIn)
                {
                    connectFromCons = toIn.GetAllConnections();
                    connectTo = fromOut.Con.GetConnectedInputs().ToArray();
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
                        connection.From.ConnectToInput(input, false, false, true);
                    }
                }
            }
            
            //After bypassing, the io shouldn't be connected to anything
            Debug.Assert(bypassFromIO.All(x => !x.IsConnectedToAnything()));
            Debug.Assert(bypassToIO.All(x => !x.IsConnectedToAnything()));
        }

        public static void BiDirFullyConnectIO(FIRIO a, FIRIO b, bool isConditional = false)
        {
            ScalarIO[] aFlat = a.Flatten().ToArray();
            ScalarIO[] bFlat = b.Flatten().ToArray();

            if (aFlat.Length != bFlat.Length)
            {
                throw new Exception($"Can't fully connect {nameof(a)} and {nameof(b)} because they do not contain the same number of IO.");
            }

            for (int i = 0; i < aFlat.Length; i++)
            {
                FIRIO aIO = aFlat[i];
                FIRIO bIO = bFlat[i];

                if (aIO is Input aIn && bIO is Output bOut)
                {
                    bOut.ConnectToInput(aIn, false, false, isConditional);
                }
                else if (aIO is Output aOut && bIO is Input bIn)
                {
                    aOut.ConnectToInput(bIn, false, false, isConditional);
                }
                else
                {
                    throw new Exception($"Can't connect IO of type {a.GetType()} to {b.GetType()}.");
                }
            }
        }

        public static void OneWayOnlyConnect(FIRIO fromIO, FIRIO toIO)
        {
            ScalarIO[] fromFlat = fromIO.Flatten().ToArray();
            ScalarIO[] toFlat = toIO.Flatten().ToArray();

            if (fromFlat.Length != toFlat.Length)
            {
                throw new Exception($"Can't connect {nameof(fromIO)} to {nameof(toIO)} because they do not contain the same number of IO.");
            }

            for (int i = 0; i < fromFlat.Length; i++)
            {
                ScalarIO from = fromFlat[i];
                ScalarIO to = toFlat[i];

                if (from is Output fromOutput && to is Input toInput)
                {
                    fromOutput.ConnectToInput(toInput);
                }
            }
        }

        public static void PairIO(Dictionary<FIRIO, FIRIO> pairs, FIRIO fromIO, FIRIO toIO)
        {
            if (fromIO is ScalarIO && toIO is ScalarIO)
            {
                pairs.Add(fromIO, toIO);
                pairs.Add(toIO, fromIO);
            }
            else
            {
                var ioWalk = fromIO.WalkIOTree();
                var ioFlipWalk = toIO.WalkIOTree();
                foreach (var pair in ioWalk.Zip(ioFlipWalk))
                {
                    pairs.Add(pair.First, pair.Second);
                    pairs.Add(pair.Second, pair.First);
                }
            }
        }

        //internal static void PropegatePorts(IPortsIO fromPortIO)
        //{
        //    while (true)
        //    {
        //        FIRRTLNode fromNode = ((FIRIO)fromPortIO).Node;

        //    }


        //    //HashSet<FIRRTLNode> seenDestinations = new HashSet<FIRRTLNode>();
        //    //FIRRTLNode fromNode = ((FIRIO)fromHidden).Node;
        //    //while (seenDestinations.Add(fromNode) && fromNode is Module dstMod)// || dstNode is Wire)
        //    //{
        //    //    IHiddenPorts toHidden = (IHiddenPorts)dstMod.GetPairedIO((FIRIO)fromHidden);

        //    //    FIRIO[] fromPorts = fromHidden.GetHiddenPorts();
        //    //    FIRIO[] toPorts = toHidden.CopyHiddenPortsFrom(fromHidden);

        //    //    for (int y = 0; y < fromPorts.Length; y++)
        //    //    {
        //    //        fromPorts[y].ConnectToInput(toPorts[y]);
        //    //    }

        //    //    fromNode = ((FIRIO)toHidden).Node;
        //    //    fromHidden = toHidden;
        //    //}
        //}
    }
}