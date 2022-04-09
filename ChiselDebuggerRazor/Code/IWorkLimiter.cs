using System;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code
{
    public interface IWorkLimiter {
        Task AddWork(Func<Task> work, int priority);
    }
}
