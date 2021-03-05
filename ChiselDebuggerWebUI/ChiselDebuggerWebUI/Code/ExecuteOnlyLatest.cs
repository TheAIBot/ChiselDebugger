using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
{
    public class ExecuteOnlyLatest<T> : IDisposable
    {
        private readonly BlockingCollection<T> WorkItems = new BlockingCollection<T>(new ConcurrentQueue<T>());
        private readonly CancellationTokenSource CancelSource = new CancellationTokenSource();
        private Task Worker;

        public void Start(Action<T> workOnLatestItem)
        {
            this.Worker = Task.Factory.StartNew(async () =>
            {
                try
                {
                    var token = CancelSource.Token;

                    while (!token.IsCancellationRequested)
                    {
                        T workItem = WorkItems.Take(token);

                        //Many time changes may be queued up and the new circuit state
                        //should be set to the latest one added to the queue so the 
                        //shown circuit state is as up to date as possible
                        if (WorkItems.Count > 0)
                        {
                            workItem = WorkItems.Take(WorkItems.Count).Last();
                        }

                        workOnLatestItem(workItem);

                        //Limit how often it can update the circuit state so it
                        //doesn't end up rerendering as fast as possible.
                        await Task.Delay(10);
                    }
                }
                catch (OperationCanceledException e) { }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void AddWork(T workItem)
        {
            WorkItems.Add(workItem);
        }

        public void Dispose()
        {
            if (Worker == null)
            {
                return;
            }

            CancelSource.Cancel();
            CancelSource.Dispose();
            Worker = null;
        }
    }
}
