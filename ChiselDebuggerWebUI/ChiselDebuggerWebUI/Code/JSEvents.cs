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
    }
}
