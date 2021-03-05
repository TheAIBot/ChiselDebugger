using ChiselDebug;
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

        public delegate void DragHandler(Point dragged);
        private static readonly Dictionary<string, DragHandler> DragListeners = new Dictionary<string, DragHandler>();

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

        [JSInvokable]
        public static void DragEventAsync(string elementID, int draggedX, int draggedY)
        {
            if (DragListeners.TryGetValue(elementID, out var handler))
            {
                handler.Invoke(new Point(draggedX, draggedY));
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

        public static ValueTask AddDragListener(IJSRuntime js, string elementID, DragHandler dragHandler)
        {
            DragListeners.Add(elementID, dragHandler);
            return js.InvokeVoidAsync("JSUtils.addDragListener", elementID);
        }
    }
}
