using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Routing;
using ChiselDebug.Utilities;
using ChiselDebuggerRazor.Code;
using ChiselDebuggerRazor.Code.Controllers;
using ChiselDebuggerRazor.Code.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Components
{
    public abstract class FIRBase<T> : ManualRenderBase, IFIRUINode where T : FIRRTLNode
    {
        [Inject]
        private IJSRuntime JS { get; set; }

        [Parameter]
        public Positioned<T> PosOp { get; set; }

        [Parameter]
        public FIRLayout ParentLayoutCtrl { get; set; }

        protected Point Position => PosOp.Position;
        protected T Operation => PosOp.Value;

        private Point PreviousSize = Point.Zero;
        //private int RenderCounter = 0;
        protected readonly string SizeWatcherID = UniqueID.UniqueHTMLID();
        protected DirectedIO[] SinkOffsets = Array.Empty<DirectedIO>();
        protected DirectedIO[] SourceOffsets = Array.Empty<DirectedIO>();

        protected Point GetCurrentSize()
        {
            return PreviousSize;
        }

        protected void SetCurrentSize(Point size)
        {
            if (PreviousSize == size)
            {
                return;
            }

            PreviousSize = size;
        }

        protected override void OnFirstParametersSetAsync()
        {
            ParentLayoutCtrl?.AddUINode(this);
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                AddSizeWatcher(SizeWatcherID, JSOnResize);
            }

            return base.OnAfterRenderAsync(firstRender);

            //Debug.WriteLine($"Render: {typeof(T)} sizeChange: {sizeChanged}, Count: {RenderCounter++}");
        }

        protected void AddSizeWatcher(string componentID, JSEvents.ResizeHandler onResize)
        {
            JSEvents.BatchAddResizeListener(JS, componentID, onResize);
        }

        private Task JSOnResize(ElemWH size)
        {
            PreviousSize = size.ToPoint();
            OnResize(PreviousSize.X, PreviousSize.Y);
            return Task.CompletedTask;
        }

        public void PrepareForRender()
        {
            HasToRender = true;
        }

        protected virtual void OnResize(int width, int height)
        {
            SinkOffsets = OnMakeSinks(width, height);
            SourceOffsets = OnMakeSources(width, height);

            ParentLayoutCtrl?.UpdateComponentInfo(new FIRComponentUpdate(Operation, GetCurrentSize(), SinkOffsets, SourceOffsets));
        }

        protected virtual bool OnMove(Point newPos)
        {
            return true;
        }

        protected abstract DirectedIO[] OnMakeSinks(int width, int height);
        protected abstract DirectedIO[] OnMakeSources(int width, int height);
    }
}
