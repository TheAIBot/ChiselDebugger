using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace ChiselDebuggerWebUI.Code
{
    public static class WorkLimiter
    {
        private static readonly Dictionary<ISourceBlock<Action>, IDisposable> ActiveLinks = new Dictionary<ISourceBlock<Action>, IDisposable>();
        private static readonly ActionBlock<Action> Worker = new ActionBlock<Action>(x => x(), new ExecutionDataflowBlockOptions()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            MaxMessagesPerTask = 1
        });

        public static void LinkSource(ISourceBlock<Action> workSource)
        {
            lock (ActiveLinks)
            {
                ActiveLinks.Add(workSource, workSource.LinkTo(Worker));
            }
        }

        public static void UnlinkSource(ISourceBlock<Action> workSource)
        {
            lock (ActiveLinks)
            {
                if (ActiveLinks.Remove(workSource, out var link))
                {
                    link.Dispose();
                }
                else
                {
                    throw new Exception("Attempted to remove link that did not exist.");
                }
            }
        }
    }
}
