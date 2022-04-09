using ChiselDebuggerRazor.Code;
using Microsoft.JSInterop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebAsmUI.Code
{
    public sealed class WorkLimiter : IWorkLimiter
    {
        private static IJSRuntime JSRuntime;

        public WorkLimiter(IJSRuntime jSRuntime)
        {
            JSRuntime = jSRuntime;
        }

        private static int TotalWork = 0;
        private static readonly SortedDictionary<int, Queue<Func<Task>>> WorkQueue = new ();

        [JSInvokable]
        public static async Task FrameHasBeenAnimated()
        {
            if (WorkQueue.Sum(x => x.Value.Count) == 0)
            {
                return;
            }

            var work = WorkQueue.First(x => x.Value.Count > 0).Value.Dequeue();

            try
            {
                TotalWork--;
                await work();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
            }

            await JSRuntime.InvokeVoidAsync("WorkPacing.WaitForAnimationFrame").AsTask();
        }

        public Task AddWork(Func<Task> work, int priority)
        {
            Queue<Func<Task>> queue;
            if (!WorkQueue.TryGetValue(priority, out queue))
            {
                queue = new Queue<Func<Task>>();
                WorkQueue.Add(priority, queue);
            }

            queue.Enqueue(work);
            TotalWork++;

            if (TotalWork == 1)
            {
                return JSRuntime.InvokeVoidAsync("WorkPacing.WaitForAnimationFrame").AsTask();
            }

            return Task.CompletedTask;
        }
    }
}
