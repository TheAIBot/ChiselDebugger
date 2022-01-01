using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.CombGraph
{
    public class CombComputeOrder<T> where T : ICompute
    {
        private readonly Source[] StartOutputs;
        private T[] ComputeOrder;

        internal CombComputeOrder(Source[] startOutputs, T[] computeOrder)
        {
            this.StartOutputs = startOutputs;
            this.ComputeOrder = computeOrder;
        }

        public List<Source> ComputeAndGetChanged()
        {
            List<Source> updatedConnections = new List<Source>();
            for (int i = 0; i < ComputeOrder.Length; i++)
            {
                Source updated = ComputeOrder[i].ComputeGetIfChanged();
                if (updated != null)
                {
                    updatedConnections.Add(updated);
                }
            }

            return updatedConnections;
        }

        public void ComputeFast()
        {
            for (int i = 0; i < ComputeOrder.Length; i++)
            {
                ComputeOrder[i].Compute();
            }
        }

        public void InferTypes()
        {
            for (int i = 0; i < ComputeOrder.Length; i++)
            {
                ComputeOrder[i].InferType();
            }
        }

        public ReadOnlySpan<Source> GetAllRootSources()
        {
            return StartOutputs;
        }

        public ReadOnlySpan<T> GetComputeOrder()
        {
            return ComputeOrder;
        }

        public void SetComputeOrder(T[] order)
        {
            ComputeOrder = order;
        }

        public CombComputeOrder<T> Copy()
        {
            return new CombComputeOrder<T>(StartOutputs.ToArray(), ComputeOrder.ToArray());
        }

        public CombComputeOrder<ComputableOpti> ToOptimized()
        {
            ComputableOpti[] optimized = new ComputableOpti[ComputeOrder.Length];
            for (int i = 0; i < ComputeOrder.Length; i++)
            {
                optimized[i] = new ComputableOpti(ComputeOrder[i]);
            }

            return new CombComputeOrder<ComputableOpti>(StartOutputs.ToArray(), optimized);
        }

        public static CombComputeOrder<Computable> MakeMonoGraph(Module module)
        {
            return CreateComputeOrder.MakeMonoGraph(module);
        }
    }
}
