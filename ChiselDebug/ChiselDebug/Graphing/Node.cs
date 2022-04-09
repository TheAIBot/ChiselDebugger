using System.Collections.Generic;

namespace ChiselDebug.Graphing
{
    internal class Node<T>
    {
        public HashSet<Node<T>> Incomming { get; private set; } = new HashSet<Node<T>>();
        public HashSet<Node<T>> Outgoing { get; private set; } = new HashSet<Node<T>>();
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
}
