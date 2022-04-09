using ChiselDebug.GraphFIR.Components;

namespace ChiselDebug.Placing.GraphViz
{
    public sealed class GraphVizPlacerFactory : INodePlacerFactory
    {
        public INodePlacer Create(Module mod)
        {
            return new GraphVizPlacer(mod);
        }
    }
}
