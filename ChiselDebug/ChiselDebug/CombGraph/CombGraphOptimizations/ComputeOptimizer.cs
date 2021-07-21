using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.CombGraph.CombGraphOptimizations
{
    internal static class ComputeOptimizer
    {
        public static CombComputeOrder<ComputableOpti> Optimize(CombComputeOrder<Computable> graph)
        {
            graph.ComputeFast();

            var optimized = graph.Copy();

            ConstFolding.Optimize(optimized);
            OmitSuperfluousCons.Optimize(optimized);


            return optimized.ToOptimized();
        }
    }
}
