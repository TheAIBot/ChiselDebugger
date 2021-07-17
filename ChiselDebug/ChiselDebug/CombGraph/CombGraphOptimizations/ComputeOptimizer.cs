using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.CombGraph.CombGraphOptimizations
{
    internal static class ComputeOptimizer
    {
        public static CombComputeOrder Optimize(CombComputeOrder graph)
        {
            graph.ComputeFast();

            var optimized = graph.Copy();

            ConstFolding.Optimize(optimized);
            OmitSuperfluousCons.Optimize(optimized);

            optimized.SetIsOptimized();
            return optimized;
        }
    }
}
