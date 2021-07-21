using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;

namespace ChiselDebug.CombGraph
{
    public interface ICompute
    {
        void Compute();
        Output ComputeGetIfChanged();
        void InferType();

        FIRRTLNode GetNode();

        Output GetConnection();
    }
}
