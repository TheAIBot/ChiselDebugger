using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;

namespace ChiselDebug.CombGraph
{
    public interface ICompute
    {
        void Compute();
        Source? ComputeGetIfChanged();
        void InferType();

        FIRRTLNode? GetNode();

        Source? GetConnection();
    }
}
