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


        public Task AddWork(T workItem, Action<T> work)
        {
            lock (Locker)
            {
                WorkItem = workItem;
                if (QueuedWork)
                {
                    return Task.CompletedTask;
                }

                QueuedWork = true;
                return WorkLimiter.AddWork(() =>
                {
                    ExecuteWork(work);
                    return Task.CompletedTask;
                }, 3);
            }
        }

        private void ExecuteWork(Action<T> work)
        {
            try
            {
                T workItem;
                lock (Locker)
                {
                    workItem = WorkItem;
                }

                work(workItem);
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
