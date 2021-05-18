using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.CombGraph.CombGraphOptimizations
{
    internal static class ComputeOptimizer
    {
        public static CombComputeGraph Optimize(CombComputeGraph graph)
        {
            graph.ComputeFast();

            var optimized = graph.Copy();

            ConstFolding.Optimize(optimized);
            OmitSuperfluousCons.Optimize(optimized);

            return optimized;
        }
    }
}
