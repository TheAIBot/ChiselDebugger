using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Routing;
using ChiselDebug.Utilities;

namespace ChiselDebuggerRazor.Code
{
    public record FIRComponentUpdate(FIRRTLNode Node, Point Size, DirectedIO[] InputOffsets, DirectedIO[] OutputOffsets);
}
