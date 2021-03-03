using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
{
    public static class JSEvents
    {
        public delegate void ScrollHandler(int scrollDelta);
        public static event ScrollHandler OnWindowScroll;

        [JSInvokable]
        public static void ScrollEventAsync(int scrollDelta)
        {
            OnWindowScroll?.Invoke(scrollDelta);
        }
    }
}
