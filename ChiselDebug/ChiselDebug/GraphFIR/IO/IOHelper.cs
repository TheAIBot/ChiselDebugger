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
    }
}