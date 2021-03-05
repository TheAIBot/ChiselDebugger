using System.Threading;

namespace ChiselDebuggerWebUI.Code
{
    public static class UniqueID
    {
        private static int IDCounter = 0;
        public static string UniqueHTMLID()
        {
            return "unique--" + Interlocked.Increment(ref IDCounter);
        }
    }
}
