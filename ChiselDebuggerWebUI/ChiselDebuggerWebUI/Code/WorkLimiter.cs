using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace ChiselDebuggerWebUI.Code
{
    public static class WorkLimiter
    {
        private static readonly Dictionary<ISourceBlock<Action>, IDisposable> ActiveLinks = new Dictionary<ISourceBlock<Action>, IDisposable>();
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
