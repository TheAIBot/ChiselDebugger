using System;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code
{
    public sealed class SeqWorkOverrideOld<T>
    {
        private T WorkItem;
        private readonly object Locker = new object();
        private bool QueuedWork = false;
        private readonly IWorkLimiter WorkLimiter;

        public SeqWorkOverrideOld(IWorkLimiter workLimiter)
        {
            WorkLimiter = workLimiter;
        }


        public Task AddWorkAsync(T workItem, Func<T, Task> work)
        {
            lock (Locker)
            {
                WorkItem = workItem;
                if (QueuedWork)
                {
                    return Task.CompletedTask;
                }

                QueuedWork = true;
                return WorkLimiter.AddWorkAsync(() => ExecuteWorkAsync(work), 3);
            }
        }

        private Task ExecuteWorkAsync(Func<T, Task> work)
        {
            try
            {
                T workItem;
                lock (Locker)
                {
                    workItem = WorkItem;
                }

                return work(workItem);
            }
            finally
            {
                lock (Locker)
                {
                    QueuedWork = false;
                }
            }
        }
    }
}
