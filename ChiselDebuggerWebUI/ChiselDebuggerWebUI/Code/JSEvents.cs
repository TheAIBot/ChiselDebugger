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
        public delegate void DragHandler(Point dragged);
        private static readonly Dictionary<string, DragHandler> DragListeners = new Dictionary<string, DragHandler>();

        public delegate void ResizeHandler(ElemWH size);
        private static readonly Dictionary<string, ResizeHandler> ResizeListeners = new Dictionary<string, ResizeHandler>();

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

        public static ValueTask AddDragListener(IJSRuntime js, string elementID, DragHandler dragHandler)
        {
            DragListeners.Add(elementID, dragHandler);
            return js.InvokeVoidAsync("JSUtils.addDragListener", elementID);
        }

        [JSInvokable]
        public static void ResizeEventAsync(string elementID, ElemWH size)
        {
            if (ResizeListeners.TryGetValue(elementID, out var handler))
            {
                handler.Invoke(size);
            }
            else
            {
                throw new Exception($"No handler for element exists. Element id: {elementID}");
            }
        }

        public static ValueTask AddResizeListener(IJSRuntime js, string elementID, ResizeHandler resizeHandler)
        {
            ResizeListeners.Add(elementID, resizeHandler);
            return js.InvokeVoidAsync("JSUtils.addResizeListener", elementID);
        }
    }
}
