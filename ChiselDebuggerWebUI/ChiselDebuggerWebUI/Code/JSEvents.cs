using Microsoft.AspNetCore.Components;
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
        private static readonly Dictionary<string, ScrollHandler> ScrollListeners = new Dictionary<string, ScrollHandler>();

        [JSInvokable]
        public static void ScrollEventAsync(string elementID, int scrollDelta)
        {
            if (ScrollListeners.TryGetValue(elementID, out var handler))
            {
                handler.Invoke(scrollDelta);
            }
            else
            {
                throw new Exception($"No handler for element exists. Element id: {elementID}");
            }
        }

        public static ValueTask AddScrollListener(IJSRuntime js, string elementID, ScrollHandler scrollHandler)
        {
            ScrollListeners.Add(elementID, scrollHandler);
            return js.InvokeVoidAsync("JSUtils.addScrollListener", elementID);
        }
    }
}
