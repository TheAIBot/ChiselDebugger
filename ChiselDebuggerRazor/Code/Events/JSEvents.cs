using ChiselDebug.Utilities;
using Microsoft.JSInterop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Events
{
    public static class JSEvents
    {
        public delegate void DragHandler(Point dragged);
        private static readonly Dictionary<string, DragHandler> DragListeners = new Dictionary<string, DragHandler>();

        public delegate Task ResizeHandler(ElemWH size);
        private static readonly ConcurrentDictionary<string, ResizeHandler> ResizeListeners = new ConcurrentDictionary<string, ResizeHandler>();

        private static readonly ConcurrentQueue<(IJSRuntime js, string id)> ToAddResizeIDs = new ConcurrentQueue<(IJSRuntime js, string id)>();

        [JSInvokable]
        public static ValueTask DragEventAsync(string elementID, int draggedX, int draggedY)
        {
            if (DragListeners.TryGetValue(elementID, out var handler))
            {
                handler.Invoke(new Point(draggedX, draggedY));
            }
            else
            {
                throw new Exception($"No handler for element exists. Element id: {elementID}");
            }

            return ValueTask.CompletedTask;
        }

        public static ValueTask AddDragListenerAsync(IJSRuntime js, string elementID, DragHandler dragHandler)
        {
            if (DragListeners.TryAdd(elementID, dragHandler))
            {
                return js.InvokeVoidAsync("JSUtils.addDragListener", elementID);
            }

            return ValueTask.CompletedTask;
        }

        [JSInvokable]
        public static async Task ResizeEventsAsync(ElemResizes resizes)
        {
            for (int i = 0; i < resizes.IDs.Length; i++)
            {
                string elementID = resizes.IDs[i];
                ElemWH size = resizes.Sizes[i];

                if (ResizeListeners.TryGetValue(elementID, out var handler))
                {
                    await handler.Invoke(size);
                }
                else
                {
                    throw new Exception($"No handler for element exists. Element id: {elementID}");
                }
            }
        }

        public static void BatchAddResizeListener(IJSRuntime js, string elementID, ResizeHandler resizeHandler)
        {
            if (!ResizeListeners.TryAdd(elementID, resizeHandler))
            {
                throw new Exception("Failed to add resize event listener.");
            }

            ToAddResizeIDs.Enqueue((js, elementID));
        }

        public static ValueTask AddResizeListenerAsync(IJSRuntime js, string elementID, ResizeHandler resizeHandler)
        {
            if (ResizeListeners.TryAdd(elementID, resizeHandler))
            {
                ToAddResizeIDs.Enqueue((js, elementID));
                return DoBatchedResizeJSAsync();
            }

            return ValueTask.CompletedTask;
        }

        public static async ValueTask DoBatchedResizeJSAsync()
        {
            List<(IJSRuntime js, string id)> workItems = new List<(IJSRuntime js, string id)>();
            while (ToAddResizeIDs.TryDequeue(out var workItem))
            {
                workItems.Add(workItem);
            }

            //Work can lie in different js runtimes. Make sure that
            //the work is executed in the correct runtime by grouping
            //the work by runtime.
            foreach (var jsGroup in workItems.GroupBy(x => x.js))
            {
                string[] ids = jsGroup.Select(x => x.id).ToArray();

                //Only so much data can be sent to/from js at a time
                //which is why the work is split up here
                foreach (var idChunk in ids.Chunk(500))
                {
                    await jsGroup.Key.InvokeVoidAsync("JSUtils.addResizeListeners", new string[][] { idChunk });
                }
            }
        }
    }
}
