using ChiselDebug.GraphFIR.Components;

namespace ChiselDebug.Placing
{
    public interface INodePlacerFactory
    {
        INodePlacer Create(Module mod);
    }
}
