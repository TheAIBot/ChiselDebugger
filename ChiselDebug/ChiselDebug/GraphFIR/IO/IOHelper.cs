using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public static class IOHelper
    {
        public static void BypassWire(Sink[] bypassFrom, Source[] bypassTo)
        {
            if (bypassFrom.Length != bypassTo.Length)
            {
                throw new Exception($"Can't bypass io when they are not of the same size. From: {bypassFrom.Length}, To: {bypassTo.Length}");
            }

            for (int i = 0; i < bypassFrom.Length; i++)
            {
                Sink from = bypassFrom[i];
                Source to = bypassTo[i];

                Connection[] connectFromCons = from.GetConnections();
                Sink[] connectTo = to.GetConnectedInputs().ToArray();
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

        public static (Source output, Sink input) OrderIO(ScalarIO a, ScalarIO b) => (a, b) switch
        {
            (Source aOut, Sink bIn) => (aOut, bIn),
            (Sink aIn, Source bOut) => (bOut, aIn),
            _ => throw new Exception()
        };

        public static HashSet<AggregateIO> GetAllAggregateIOs(IEnumerable<ScalarIO> scalarIOs)
        {
            HashSet<AggregateIO> allAggIO = new HashSet<AggregateIO>();
            foreach (var scalarIO in scalarIOs)
            {
                var currentParent = scalarIO.ParentIO;
                while (currentParent != null && allAggIO.Add(currentParent))
                {
                    currentParent = currentParent.ParentIO;
                }
            }

            return allAggIO;
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

            FIRIO dataInput = port.GetSink();

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