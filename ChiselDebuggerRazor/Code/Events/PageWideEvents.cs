using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Events
{
    public sealed class PageWideEvents
    {
        public delegate Task MouseEventHandler(MouseEventArgs args);
        public event MouseEventHandler OnMouseUp;
        public event MouseEventHandler OnMouseMove;

        public Task InvokeOnMouseUp(MouseEventArgs args)
        {
            return OnMouseUp?.Invoke(args);
        }

        public Task InvokeOnMouseMove(MouseEventArgs args)
        {
            return OnMouseMove?.Invoke(args);
        }
    }
}
