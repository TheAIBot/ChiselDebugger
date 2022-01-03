using System;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace ChiselDebuggerRazor.Code
{
    public static class WorkLimiter
    {
        private static readonly ActionBlock<Action> Worker = new ActionBlock<Action>(x =>
        {
            try
            {
                x();
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

        public static void AddWork(Action work)
        {
            Worker.Post(work);
        }
    }
}
