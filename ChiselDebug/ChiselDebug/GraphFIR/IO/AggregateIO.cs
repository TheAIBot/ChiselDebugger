using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class AggregateIO : FIRIO 
    { 
        public AggregateIO(FIRRTLNode node, string name) : base(node, name) { }

        public override FIRIO GetInput()
        {
            return this;
        }

        public override FIRIO GetOutput()
        {
            return this;
        }

        public abstract FIRIO[] GetIOInOrder();

        public List<AggregateIO> GetAllOrderlyConnectedEndpoints()
        {
            List<AggregateIO> orderlyConnected = new List<AggregateIO>();
            FIRIO[] ioInOrder = GetIOInOrder();
            if (ioInOrder.Length == 0)
            {
                return orderlyConnected;
            }

            HashSet<AggregateIO> potentialEndpoints = GetAllParentIOEndpoints(ioInOrder[0]).ToHashSet();
            for (int i = 1; i < ioInOrder.Length; i++)
            {
                potentialEndpoints.IntersectWith(GetAllParentIOEndpoints(ioInOrder[i]));
            }

            List<ScalarIO> flattenedIO = Flatten();
            foreach (var potentialEndpoint in potentialEndpoints)
            {
                if (potentialEndpoint.GetType() != this.GetType())
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
                return aggIO.GetAllOrderlyConnectedEndpoints().Select(x => x.ParentIO).Where(x => x != null);
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
