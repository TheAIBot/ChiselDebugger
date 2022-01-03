using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.Transformations
{
    internal static class MakeAggregateConnections
    {
        internal static void Transform(Module mod)
        {
            List<ScalarIO> allScalarIO = mod.GetAllNestedNodesOfType<Module>()
                                            .SelectMany(x => x.GetAllNodesIncludeModule())
                                            .SelectMany(x => x.GetIO())
                                            .FlattenMany()
                                            .ToList();

            HashSet<AggregateIO> allAggIO = IOHelper.GetAllAggregateIOs(allScalarIO);
            foreach (var aggIO in allAggIO)
            {
                foreach (var connectedAggIO in GetAllOrderlyConnectedEndpoints(aggIO))
                {
                    aggIO.AddConnection(connectedAggIO.To, connectedAggIO.Condition);
                }
            }
        }

        private static List<AggregateConnection> GetAllOrderlyConnectedEndpoints(AggregateIO aggIO)
        {
            List<AggregateConnection> orderlyConnected = new List<AggregateConnection>();
            FIRIO[] ioInOrder = aggIO.GetIOInOrder();
            if (ioInOrder.Length == 0)
            {
                return orderlyConnected;
            }

            HashSet<AggregateConnection> potentialEndpoints = GetAllParentIOEndpoints(ioInOrder[0]).ToHashSet();
            for (int i = 1; i < ioInOrder.Length; i++)
            {
                potentialEndpoints.IntersectWith(GetAllParentIOEndpoints(ioInOrder[i]));
            }

            ScalarIO[] flattenedIO = aggIO.Flatten();
            foreach (var potentialEndpoint in potentialEndpoints)
            {
                if (potentialEndpoint.To.GetType() != aggIO.GetType())
                {
                    continue;
                }

                ScalarIO[] potentialEndpointIOs = potentialEndpoint.To.Flatten();
                if (potentialEndpointIOs.Length != flattenedIO.Length)
                {
                    continue;
                }

                bool hasConnectionsInOrder = true;
                for (int i = 0; i < flattenedIO.Length; i++)
                {
                    var fromTo = IOHelper.OrderIO(flattenedIO[i], potentialEndpointIOs[i]);
                    if (!fromTo.input.GetConnections().Any(x => x.From == fromTo.output && x.Condition == potentialEndpoint.Condition))
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

        private static IEnumerable<AggregateConnection> GetAllParentIOEndpoints(FIRIO io)
        {
            if (io is ScalarIO scalar)
            {
                return GetAllEndpointConnections(scalar);
            }
            else if (io is AggregateIO aggIO)
            {
                return GetAllOrderlyConnectedEndpoints(aggIO);
            }

            throw new Exception($"Unknown type of {io}");
        }

        private static IEnumerable<AggregateConnection> GetAllEndpointConnections(ScalarIO scalar)
        {
            if (scalar is Sink input)
            {
                foreach (var connection in input.GetConnections())
                {
                    if (!connection.From.IsPartOfAggregateIO)
                    {
                        continue;
                    }

                    yield return new AggregateConnection(connection.From.ParentIO, connection.Condition);
                }
            }
            else if (scalar is Source output)
            {
                foreach (var conInputs in output.GetConnectedInputs())
                {
                    if (!conInputs.IsPartOfAggregateIO)
                    {
                        continue;
                    }

                    foreach (var connection in conInputs.GetConnections())
                    {
                        if (connection.From != output)
                        {
                            continue;
                        }

                        yield return new AggregateConnection(conInputs.ParentIO, connection.Condition);
                    }
                }
            }
            else
            {
                throw new Exception("Invalid scalar type");
            }
        }
    }
}
