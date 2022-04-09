using ChiselDebug.GraphFIR.IO;
using ChiselDebuggerRazor.Pages.FIRRTLUI.IOUI;
using Microsoft.AspNetCore.Components.Web;

namespace ChiselDebuggerRazor.Code.Events
{
    public sealed class IOWindowEvents
    {
        private IOWindowUI IOWindow = null;

        public void SetIOWindow(IOWindowUI window)
        {
            IOWindow = window;
        }

        public void MouseEntersIO(FIRIO io, MouseEventArgs args)
        {
            IOWindow?.MouseEnter(io, args);
        }

        public void MouseExitIO(FIRIO io, MouseEventArgs args)
        {
            IOWindow?.MouseExit(io, args);
        }
    }
}
