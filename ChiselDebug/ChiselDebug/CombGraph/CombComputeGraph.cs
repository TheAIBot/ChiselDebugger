using ChiselDebug.GraphFIR;
using System.Collections.Generic;

namespace ChiselDebug.CombGraph
{
    public class CombComputeGraph
    {
        private readonly Dictionary<FIRRTLNode, CombComputeNode> Nodes = new Dictionary<FIRRTLNode, CombComputeNode>();

        public void AddNode(FIRRTLNode firNode, CombComputeNode combNode)
        {
            Nodes.Add(firNode, combNode);
        }

        public void AddEdge(FIRRTLNode from, FIRRTLNode to)
        {
            Nodes[from].AddEdgeTo(Nodes[to]);
        }

        public void ComputeRoots()
        {

        }

        public void ComputeGraph()
        {

        }

        public void Reset()
        {
            foreach (var node in Nodes.Values)
            {
                node.ResetRemainingDependencies();
            }
        }
    }
}
