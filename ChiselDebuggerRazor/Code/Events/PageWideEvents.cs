using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Events
{
    public sealed class PageWideEvents
    {
        public delegate Task MouseEventHandler(MouseEventArgs args);
        public event MouseEventHandler? OnMouseUp;
        public event MouseEventHandler? OnMouseMove;

        public Task InvokeOnMouseUpAsync(MouseEventArgs args)
        {
            return OnMouseUp?.Invoke(args) ?? Task.CompletedTask;
        }

        public Task InvokeOnMouseMoveAsync(MouseEventArgs args)
        {
            return OnMouseMove?.Invoke(args) ?? Task.CompletedTask;
        }
    }
}
