using Microsoft.AspNetCore.Components.Web;

namespace ChiselDebuggerRazor.Code.Events
{
    public sealed class PageWideEvents
    {
        public delegate void MouseEventHandler(MouseEventArgs args);
        public event MouseEventHandler OnMouseUp;
        public event MouseEventHandler OnMouseMove;

        public void InvokeOnMouseUp(MouseEventArgs args)
        {
            OnMouseUp?.Invoke(args);
        }

        public void InvokeOnMouseMove(MouseEventArgs args)
        {
            OnMouseMove?.Invoke(args);
        }
    }
}
