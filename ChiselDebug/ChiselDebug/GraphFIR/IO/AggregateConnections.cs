using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR.IO
{
    internal static class AggregateConnections
    {
        internal static void ConnectionAllAggregateIO(Module mod)
        {
            List<ScalarIO> allScalarIO = mod.GetAllNestedNodesOfType<Module>()
                                            .SelectMany(x => x.GetAllNodesIncludeModule())
                                            .SelectMany(x => x.GetIO())
                                            .SelectMany(x => x.Flatten())
                                            .ToList();
            
            HashSet<AggregateIO> allAggIO = new HashSet<AggregateIO>();
            foreach (var scalarIO in allScalarIO)
            {
                var currentParent = scalarIO.ParentIO;
                while (currentParent != null && allAggIO.Add(currentParent))
                {
                    currentParent = currentParent.ParentIO;
                }
            }

            foreach (var aggIO in allAggIO)
            {
                foreach (var connectedAggIO in GetAllOrderlyConnectedEndpoints(aggIO))
                {
                    aggIO.AddConnection(connectedAggIO);
                }
            }
        }

        private static List<AggregateIO> GetAllOrderlyConnectedEndpoints(AggregateIO aggIO)
        {
            List<AggregateIO> orderlyConnected = new List<AggregateIO>();
            FIRIO[] ioInOrder = aggIO.GetIOInOrder();
            if (ioInOrder.Length == 0)
            {
                return orderlyConnected;
            }

            HashSet<AggregateIO> potentialEndpoints = GetAllParentIOEndpoints(ioInOrder[0]).ToHashSet();
            for (int i = 1; i < ioInOrder.Length; i++)
            {
                potentialEndpoints.IntersectWith(GetAllParentIOEndpoints(ioInOrder[i]));
            }

            List<ScalarIO> flattenedIO = aggIO.Flatten();
            foreach (var potentialEndpoint in potentialEndpoints)
            {
                if (potentialEndpoint.GetType() != aggIO.GetType())
                {
                    continue;
                }

                List<ScalarIO> potentialEndpointIOs = potentialEndpoint.Flatten();
                if (potentialEndpointIOs.Count != flattenedIO.Count)
                {
                    continue;
                }

                bool hasConnectionsInOrder = true;
                for (int i = 0; i < flattenedIO.Count; i++)
                {
                    var fromTo = IOHelper.OrderIO(flattenedIO[i], potentialEndpointIOs[i]);
                    if (!fromTo.output.GetConnectedInputs().Contains(fromTo.input))
                    {
                        hasConnectionsInOrder = false;
                        break;
                    }
                }

                if (hasConnectionsInOrder)
                {
                    orderlyConnected.Add(potentialEndpoint);
                }
            }

            return orderlyConnected;
        }

        private static IEnumerable<AggregateIO> GetAllParentIOEndpoints(FIRIO io)
        {
            if (io is ScalarIO scalar)
            {
                return GetAllEndpointConnections(scalar).Select(x => x.ParentIO).Where(x => x != null);
            }
            else if (io is AggregateIO aggIO)
            {
                return GetAllOrderlyConnectedEndpoints(aggIO).Select(x => x.ParentIO).Where(x => x != null);
            }

            throw new Exception($"Unknown type of {io}");
        }

        private static IEnumerable<ScalarIO> GetAllEndpointConnections(ScalarIO scalar)
        {
            if (scalar is Input input)
            {
                return input.GetConnections().Select(x => x.From);
            }
            else if (scalar is Output output)
            {
                return output.GetConnectedInputs();
            }
            else
            {
                throw new Exception("Invalid scalar type");
            }
        }
    }
}
