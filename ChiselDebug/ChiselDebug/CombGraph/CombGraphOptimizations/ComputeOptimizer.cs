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
            MergeStates.Optimize(optimized);


            return optimized.ToOptimized();
        }
    }
}
