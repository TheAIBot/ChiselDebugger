using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.Graphing
{
    internal class Graph<T>
    {
        public readonly List<Node<T>> Nodes = new List<Node<T>>();

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

        public void InvertAllEdges()
        {
            foreach (var node in Nodes)
            {
                node.InvertEdges();
            }
        }

        /// <summary>
        /// Able to sort cyclic graphs
        /// </summary>
        /// <returns></returns>
        public Dictionary<Node<T>, int> TopologicalSort()
        {
            Dictionary<Node<T>, int> nodeOrders = new Dictionary<Node<T>, int>();
            foreach (var node in Nodes)
            {
                nodeOrders.Add(node, int.MinValue);
            }

            HashSet<Node<T>> inPath = new HashSet<Node<T>>();
            Stack<(Node<T> node, int depth)> path = new Stack<(Node<T> node, int depth)>();

            foreach (var noParents in Nodes.Where(x => x.Outgoing.Count == 0))
            {
                path.Push((noParents, 0));

                while (path.Count > 0)
                {
                    (Node<T> node, int depth) = path.Peek();
                    nodeOrders[node] = depth;

                    

                    bool newChild = false;
                    foreach (var child in node.Incomming)
                    {
                        //No loops in search
                        if (inPath.Contains(child))
                        {
                            continue;
                        }

                        if (nodeOrders[child] >= depth + 1)
                        {
                            continue;
                        }

                        newChild = true;
                        path.Push((child, depth + 1));
                        inPath.Add(child);
                    }

                    if (!newChild)
                    {
                        inPath.Remove(path.Pop().node);
                    }
                }
            }

            int maxOrder = nodeOrders.Values.Max();
            foreach (var node in nodeOrders.Keys)
            {
                nodeOrders[node] = maxOrder - nodeOrders[node];
            }

            return nodeOrders;
        }
    }
}
