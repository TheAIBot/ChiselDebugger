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
        public ModuleController ParentModCtrl { get; set; }

        [CascadingParameter(Name = "DebugCtrl")]
        protected DebugController DebugCtrl { get; set; }

        protected Point Position => PosOp.Position;
        protected T Operation => PosOp.Value;

        private Point PreviousSize = new Point(0, 0);
        private Point PreviousPos = new Point(0, 0);
        private int RenderCounter = 0;
        private bool IsFirstSetParametersEvent = true;
        protected ElementReference SizeWatcher;
        protected List<DirectedIO> InputOffsets = new List<DirectedIO>();
        protected List<DirectedIO> OutputOffsets = new List<DirectedIO>();

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

        protected override Task OnParametersSetAsync()
        {
            if (IsFirstSetParametersEvent)
            {
                IsFirstSetParametersEvent = false;

                DebugCtrl.AddUINode(this, Operation.GetInputs().Select(x => x.Con).Where(x => x != null).ToList());
                DebugCtrl.AddUINode(this, Operation.GetOutputs().Select(x => x.Con).ToList());
                ParentModCtrl?.AddUINode(this);
                OnFirstParametersSetAsync();
            }
            return base.OnParametersSetAsync();
        }

        protected virtual void OnFirstParametersSetAsync()
        { }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            ElemWH elemWH = await JS.InvokeAsync<ElemWH>("JSUtils.getElementSize", SizeWatcher);
            Point newSize = elemWH.ToPoint();

            if (firstRender)
            {
                OnAfterFirstRenderAsync(newSize.X, newSize.Y);
            }

            bool sizeChanged = PreviousSize != newSize;
            PreviousSize = newSize;
            if (sizeChanged)
            {
                if (OnResize(newSize.X, newSize.Y))
                {
                    StateHasChanged();
                }
            }

            bool hasMoved = PreviousPos != Position;
            PreviousPos = Position;
            if (hasMoved)
            {
                if (OnMove(Position))
                {
                    StateHasChanged();
                }
            }

            //Debug.WriteLine($"Render: {typeof(T)} sizeChange: {sizeChanged}, posChange: {hasMoved}, Count: {RenderCounter++}");
        }

        public void PrepareForRender()
        {
            HasToRender = true;
        }

        protected virtual void OnAfterFirstRenderAsync(int width, int height)
        { }

        protected virtual bool OnResize(int width, int height)
        {
            InputOffsets = OnMakeInputs(width, height);
            OutputOffsets = OnMakeOutputs(width, height);

            ParentModCtrl?.UpdateComponentInfo(new FIRComponentUpdate(Operation, new Point(width, height), InputOffsets, OutputOffsets));

            return true;
        }

        protected virtual bool OnMove(Point newPos)
        {
            return true;
        }

        protected abstract List<DirectedIO> OnMakeInputs(int width, int height);
        protected abstract List<DirectedIO> OnMakeOutputs(int width, int height);
    }
}
