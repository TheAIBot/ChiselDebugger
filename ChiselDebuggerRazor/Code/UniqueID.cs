using System.Threading;

namespace ChiselDebuggerRazor.Code
{
    public static class UniqueID
    {
        private static int IDCounter = 0;
        public static string UniqueHTMLID()
        {
            return $"u-{Interlocked.Increment(ref IDCounter)}";
        }
    }
}
