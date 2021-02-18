using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.Graphing
{
    internal class Node<T>
    {
        public readonly HashSet<Node<T>> Incomming = new HashSet<Node<T>>();
        public readonly HashSet<Node<T>> Outgoing = new HashSet<Node<T>>();
        public readonly T Value;

        public Node(T value)
        {
            this.Value = value;
        }

        public void AddEdgeTo(Node<T> node)
        {
            Outgoing.Add(node);
            node.Incomming.Add(this);
        }

        public void RemoveEdgeTo(Node<T> node)
        {
            Outgoing.Remove(node);
            node.Incomming.Remove(this);
        }
    }

    internal class Graph<T>
    {
        List<Node<T>> Nodes = new List<Node<T>>();

        public void AddNode(Node<T> node)
        {
            Nodes.Add(node);
        }

        public void AddEdge(Node<T> from, Node<T> to)
        {
            from.AddEdgeTo(to);
        }

        public void RemoveNode(Node<T> node)
        {
            Nodes.Remove(node);

            foreach (var parentNode in node.Incomming)
            {
                parentNode.RemoveEdgeTo(node);
            }
            foreach (var childNode in node.Outgoing)
            {
                node.RemoveEdgeTo(childNode);
            }
        }

        private class TopologicalData
        {
            public int Ordering = 0;
            public int ParentsNotDone = 0;
            public bool IsDone = false;
        }

        /// <summary>
        /// Able to sort cyclic graphs
        /// </summary>
        /// <returns></returns>
        public Dictionary<Node<T>, int> TopologicalSort()
        {
            List<Node<T>> remaining = new List<Node<T>>();
            var nodeData = new Dictionary<Node<T>, TopologicalData>();

            //Make extra data for each node
            foreach (var node in Nodes)
            {
                remaining.Add(node);
                nodeData.Add(node, new TopologicalData());
            }

            //For each node count how many of its parents
            //aren't ready
            foreach (var node in remaining)
            {
                foreach (var childNode in node.Outgoing)
                {
                    nodeData[childNode].ParentsNotDone++;
                }
            }

            while (remaining.Count > 0)
            {
                remaining.Sort((x, y) => nodeData[x].ParentsNotDone - nodeData[y].ParentsNotDone);

                Node<T> mostReady;
                //Due to cyclic graph, it may be the case that there are nodes left
                //but none have all their parent ready.
                int firstNodeParentsNotDone = nodeData[remaining.First()].ParentsNotDone;
                if (firstNodeParentsNotDone == 0)
                {
                    mostReady = remaining.First();
                    remaining.RemoveAt(0);
                }
                else
                {
                    //Chose from one of the nodes that are most prepared.
                    //Chose from one of the nodes who has a parent with the lowest
                    //ordering value.
                    mostReady = remaining
                        .Where(x => nodeData[x].ParentsNotDone == firstNodeParentsNotDone)
                        .OrderBy(x => x.Incomming.Min(x => nodeData[x].Ordering)).First();
                    remaining.Remove(mostReady);
                }

                int ordering = 0;
                if (mostReady.Incomming.Count > 0)
                {
                    //int sumParentsOrder = mostReady.Incomming.Sum(x => nodeData[x].Ordering);
                    //float meanParentOrder = sumParentsOrder / (float)mostReady.Incomming.Count;
                    //ordering = (int)MathF.Round(meanParentOrder + 0.25f) + 1;
                    ordering = mostReady.Incomming.Max(x => nodeData[x].Ordering) + 1;
                }

                nodeData[mostReady].Ordering = ordering;
                foreach (var childNode in mostReady.Outgoing)
                {
                    nodeData[childNode].ParentsNotDone--;
                }
            }

            Dictionary<Node<T>, int> nodeOrders = new Dictionary<Node<T>, int>();
            foreach (var keyValue in nodeData)
            {
                nodeOrders.Add(keyValue.Key, keyValue.Value.Ordering);
            }

            return nodeOrders;
        }
    }
}
