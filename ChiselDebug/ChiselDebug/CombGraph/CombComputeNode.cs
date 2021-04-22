using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;

namespace ChiselDebug.CombGraph
{
    public class CombComputeNode
    {
        private readonly Output[] StartOutputs;
        private readonly Input[] StopInputs;
        private readonly Computable[] ComputeOrder;
        private readonly Connection[] ConsResponsibleFor;
        private CombComputeNode[] OutgoingEdges;
        private int TotalComputeDependencies = 0;
        private int RemainingComputeDependencies = 0;

        public CombComputeNode(Output[] startOutputs, Input[] stopInputs, Computable[] computeOrder, Connection[] consResponsibleFor)
        {
            this.StartOutputs = startOutputs;
            this.StopInputs = stopInputs;
            this.ComputeOrder = computeOrder;
            this.ConsResponsibleFor = consResponsibleFor;
        }

        public void AddEdges(List<CombComputeNode> edgesTo)
        {
            OutgoingEdges = new CombComputeNode[edgesTo.Count];
            for (int i = 0; i < edgesTo.Count; i++)
            {
                OutgoingEdges[i] = edgesTo[i];
                edgesTo[i].AddComputeDependency();
            }
        }

        private void AddComputeDependency()
        {
            TotalComputeDependencies++;
        }

        public bool IsWaitingForDependencies()
        {
            return RemainingComputeDependencies > 0;
        }

        public List<Connection> Compute()
        {
            List<Connection> updatedConnections = new List<Connection>();
            foreach (var compute in ComputeOrder)
            {
                Connection updated = compute.Compute();
                if (updated != null)
                {
                    updatedConnections.Add(updated);
                }
            }

            foreach (var edge in OutgoingEdges)
            {
                edge.RemainingComputeDependencies--;
            }

            return updatedConnections;
        }

        public void ResetRemainingDependencies()
        {
            RemainingComputeDependencies = TotalComputeDependencies;
        }

        public bool HasComputeDependencies()
        {
            return TotalComputeDependencies > 0;
        }

        public ReadOnlySpan<Output> GetStartOutputs()
        {
            return StartOutputs.AsSpan();
        }

        public ReadOnlySpan<Input> GetStopInputs()
        {
            return StopInputs.AsSpan();
        }

        public ReadOnlySpan<Connection> GetResponsibleConnections()
        {
            return ConsResponsibleFor.AsSpan();
        }

        public ReadOnlySpan<CombComputeNode> GetEdges()
        {
            return OutgoingEdges.AsSpan();
        }
    }
}
