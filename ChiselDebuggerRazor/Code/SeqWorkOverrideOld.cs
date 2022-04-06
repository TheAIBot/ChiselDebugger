using System;

namespace ChiselDebuggerRazor.Code
{
    public class SeqWorkOverrideOld<T>
    {
        private T WorkItem;
        private readonly object Locker = new object();
        private bool QueuedWork = false;
        private readonly WorkLimiter WorkLimiter;

        public SeqWorkOverrideOld(WorkLimiter workLimiter)
        {
            WorkLimiter = workLimiter;
        }


        public void AddWork(T workItem, Action<T> work)
        {
            lock (Locker)
            {
                WorkItem = workItem;
                if (QueuedWork)
                {
                    return;
                }

                QueuedWork = true;
                WorkLimiter.AddWork(() => ExecuteWork(work));
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
