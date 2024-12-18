using ChiselDebuggerRazor.Code;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ChiselDebuggerWebUI.Code
{
    public sealed class WorkLimiter : IWorkLimiter
    {
        private static readonly ActionBlock<Func<Task>> Worker = new ActionBlock<Func<Task>>(async x =>
        {
            try
            {
                await x();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
            }
        }, new ExecutionDataflowBlockOptions()
        {
            //Leave two processors for UI updates
            MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 2),
            MaxMessagesPerTask = 1
        });

        public Task AddWorkAsync(Func<Task> work, int priority)
        {
            return Worker.SendAsync(work);
        }
    }
}
