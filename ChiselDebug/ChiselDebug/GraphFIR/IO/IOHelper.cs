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
            Input[] inputs = from.Con.To.ToArray();
            foreach (var input in inputs)
            {
                from.Con.DisconnectInput(input);
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
                    connectFromCons = fromIn.GetAllConnections();
                    connectTo = toOut.Con.To.ToArray();
                    fromIn.DisconnectAll();
                }
                else if (bypassFromIO[i] is Output fromOut && bypassToIO[i] is Input toIn)
                {
                    connectFromCons = toIn.GetAllConnections();
                    connectTo = fromOut.Con.To.ToArray();
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

        public static void BiDirFullyConnectIO(FIRIO a, FIRIO b)
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
                    bOut.ConnectToInput(aIn);
                }
                else if (aIO is Output aOut && bIO is Input bIn)
                {
                    aOut.ConnectToInput(bIn);
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
    }
}