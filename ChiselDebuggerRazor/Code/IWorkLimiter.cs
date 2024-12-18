using System;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code
{
    public interface IWorkLimiter
    {
        Task AddWorkAsync(Func<Task> work, int priority);
    }
}
