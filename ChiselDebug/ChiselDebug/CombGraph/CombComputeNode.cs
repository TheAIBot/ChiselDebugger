﻿using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System.Collections.Generic;

namespace ChiselDebug.CombGraph
{
    public class CombComputeNode
    {
        private readonly Output[] StartOutputs;
        private readonly List<FIRRTLNode> ComputeOrder;
        private readonly List<Connection> ConsResponsibleFor;
        private readonly List<CombComputeNode> OutgoingEdges = new List<CombComputeNode>();
        private int TotalComputeDependencies = 0;
        private int RemainingComputeDependencies = 0;

        public CombComputeNode(Output[] startOutputs, List<FIRRTLNode> computeOrder, List<Connection> consResponsibleFor)
        {
            this.StartOutputs = startOutputs;
            this.ComputeOrder = computeOrder;
            this.ConsResponsibleFor = consResponsibleFor;
        }

        public void AddEdgeTo(CombComputeNode edgeTo)
        {
            OutgoingEdges.Add(edgeTo);
            edgeTo.AddComputeDependency();
        }

        private void AddComputeDependency()
        {
            TotalComputeDependencies++;
        }

        public bool IsWaitingForDependencies()
        {
            return RemainingComputeDependencies > 0;
        }

        public void Compute()
        {

        }

        public void ResetRemainingDependencies()
        {
            RemainingComputeDependencies = TotalComputeDependencies;
        }

        public Connection[] GetResponsibleConnections()
        {
            return ConsResponsibleFor.ToArray();
        }
    }
}
