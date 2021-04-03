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
                Output connectFrom;
                Input[] connectTo;

                //They must both either be connected or not.
                //Can't bypass when only input or output is connected.
                if (bypassFromIO[i].IsConnected() ^ bypassToIO[i].IsConnected())
                {
                    throw new Exception("Bypass IO must either both be connected or disconnected.");
                }

                if (bypassFromIO[i] is Input fromIn && bypassToIO[i] is Output toOut)
                {
                    connectFrom = fromIn.Con.From;
                    connectTo = toOut.Con.To.ToArray();
                    fromIn.Con.DisconnectInput(fromIn);
                }
                else if (bypassFromIO[i] is Output fromOut && bypassToIO[i] is Input toIn)
                {
                    connectFrom = toIn.Con.From;
                    connectTo = fromOut.Con.To.ToArray();
                    toIn.Con.DisconnectInput(toIn);
                }
                else 
                {
                    throw new Exception($"Bypass must go from Output to Input. IO is {bypassFromIO[i]} and {bypassToIO[i]}");
                }
                
                foreach (var input in connectTo)
                {
                    input.Con.DisconnectInput(input);
                    connectFrom.ConnectToInput(input);
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
    }
}