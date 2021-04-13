using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebuggerWebUI.Code;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Components
{
    public interface IFIRUINode
    {
        public abstract void PrepareForRender();
    }
    public abstract class FIRBase<T> : ManualRenderBase, IFIRUINode where T : FIRRTLNode
    {
        [Inject]
        private IJSRuntime JS { get; set; }

        [Parameter]
        public Positioned<T> PosOp { get; set; }

        [Parameter]
        public FIRLayout ParentLayoutCtrl { get; set; }

        [CascadingParameter(Name = "DebugCtrl")]
        protected DebugController DebugCtrl { get; set; }

        protected Point Position => PosOp.Position;
        protected T Operation => PosOp.Value;

        private Point PreviousSize = Point.Zero;
        private int RenderCounter = 0;
        protected readonly string SizeWatcherID = UniqueID.UniqueHTMLID();
        protected List<DirectedIO> InputOffsets = new List<DirectedIO>();
        protected List<DirectedIO> OutputOffsets = new List<DirectedIO>();
        private readonly List<string> SizeWatchIDs = new List<string>();

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
            JSEvents.AddResizeListener(JS, componentID, onResize);
            SizeWatchIDs.Add(componentID);
        }

        private void JSOnResize(ElemWH size)
        {
            PreviousSize = size.ToPoint();
            OnResize(PreviousSize.X, PreviousSize.Y);
        }

        public void PrepareForRender()
        {
            HasToRender = true;
        }

        protected virtual void OnResize(int width, int height)
        {
            InputOffsets = OnMakeInputs(width, height);
            OutputOffsets = OnMakeOutputs(width, height);

            ParentLayoutCtrl?.UpdateComponentInfo(new FIRComponentUpdate(Operation, GetCurrentSize(), InputOffsets, OutputOffsets));
        }

        protected virtual bool OnMove(Point newPos)
        {
            return true;
        }

        protected abstract List<DirectedIO> OnMakeInputs(int width, int height);
        protected abstract List<DirectedIO> OnMakeOutputs(int width, int height);
    }
}
