using ChiselDebug.GraphFIR.Components;

namespace ChiselDebug.Placing
{
    public sealed class SimplePlacerFactory : INodePlacerFactory
    {
        public INodePlacer Create(Module mod)
        {
            return new SimplePlacer(mod);
        }
    }
}
