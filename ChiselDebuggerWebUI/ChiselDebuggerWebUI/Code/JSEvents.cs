using ChiselDebug;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ChiselDebuggerWebUI.Code
{
    public static class JSEvents
    {
        public delegate void DragHandler(Point dragged);
        private static readonly Dictionary<string, DragHandler> DragListeners = new Dictionary<string, DragHandler>();

        public delegate void ResizeHandler(ElemWH size);
        private static readonly ConcurrentDictionary<string, ResizeHandler> ResizeListeners = new ConcurrentDictionary<string, ResizeHandler>();

        private static readonly ConcurrentBag<(IJSRuntime js, string id)> ToAddResizeIDs = new ConcurrentBag<(IJSRuntime js, string id)>();
        private static readonly AutoResetEvent AddedResize = new AutoResetEvent(false);

        static JSEvents()
        {
            Task.Run(DoBatchedResizeJS);
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

        public static ValueTask AddDragListener(IJSRuntime js, string elementID, DragHandler dragHandler)
        {
            DragListeners.Add(elementID, dragHandler);
            return js.InvokeVoidAsync("JSUtils.addDragListener", elementID);
        }

        [JSInvokable]
        public static void ResizeEventsAsync(ElemResizes resizes)
        {
            for (int i = 0; i < resizes.IDs.Length; i++)
            {
                string elementID = resizes.IDs[i];
                ElemWH size = resizes.Sizes[i];

                if (ResizeListeners.TryGetValue(elementID, out var handler))
                {
                    handler.Invoke(size);
                }
                else
                {
                    throw new Exception($"No handler for element exists. Element id: {elementID}");
                }
            }
        }

        public static void AddResizeListener(IJSRuntime js, string elementID, ResizeHandler resizeHandler)
        {
            if (!ResizeListeners.TryAdd(elementID, resizeHandler))
            {
                throw new Exception("Failed to add resize event listener.");
            }
            
            ToAddResizeIDs.Add((js, elementID));
            AddedResize.Set();
        }

        private static async void DoBatchedResizeJS()
        {
            while (true)
            {
                AddedResize.WaitOne();
                await Task.Delay(50);

                int count = ToAddResizeIDs.Count;
                var workItems = ToAddResizeIDs.Take(count);

                //Work can lie in different js runtimes. Make sure that
                //the work is executed in the correct runtime by grouping
                //the work by runtime.
                foreach (var jsGroup in workItems.GroupBy(x => x.js))
                {
                    string[] ids = jsGroup.Select(x => x.id).ToArray();

                    //Only so much data can be sent to/from js at a time
                    //which is why the work is split up here
                    for (int i = 0; i < ids.Length; i += 500)
                    {
                        string[] slice = new string[ids.Length - i];
                        Array.Copy(ids, i, slice, 0, slice.Length);

                        await jsGroup.Key.InvokeVoidAsync("JSUtils.addResizeListeners", new string[][] { slice });
                    }
                }
            }
        }
    }
}
