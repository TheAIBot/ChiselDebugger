using ChiselDebuggerRazor.Code;
using Microsoft.JSInterop;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
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


        private static readonly ConcurrentQueue<Func<Task>> WorkQueue = new ConcurrentQueue<Func<Task>>();

        [JSInvokable]
        public static async Task FrameHasBeenAnimated()
        {
            Func<Task> work;
            if (!WorkQueue.TryDequeue(out work))
            {
                return;
            }

            try
            {
                await work();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
            }

            await JSRuntime.InvokeVoidAsync("WorkPacing.WaitForAnimationFrame").AsTask();
        }

        public Task AddWork(Func<Task> work)
        {
            WorkQueue.Enqueue(work);

            if (WorkQueue.Count == 1)
            {
                return JSRuntime.InvokeVoidAsync("WorkPacing.WaitForAnimationFrame").AsTask();
            }

            return Task.CompletedTask;
        }
    }
}
