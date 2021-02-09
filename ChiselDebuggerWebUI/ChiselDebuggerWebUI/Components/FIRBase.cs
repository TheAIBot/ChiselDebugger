using ChiselDebug;
using ChiselDebug.FIRRTL;
using ChiselDebuggerWebUI.Code;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Components
{
    public abstract class FIRBase<T> : ComponentBase where T : FIRRTLNode
    {
        [Inject]
        private IJSRuntime JS { get; set; }

        [Parameter]
        public Positioned<T> PosOp { get; set; }

        [Parameter]
        public EventCallback<SizeUpdate> OnComponentResized { get; set; }
        [Parameter]
        public EventCallback<IOPositionUpdate> OnIOChangedPosition { get; set; }

        protected Point Position => PosOp.Position;
        protected T Operation => PosOp.Value;

        private Point PreviousSize = new Point(0, 0);
        private Point PreviousPos = new Point(0, 0);
        private bool HasUpdatedIOPosThisRender = false;
        protected ElementReference SizeWatcher;
        protected List<Positioned<Input>> InputOffsets = new List<Positioned<Input>>();
        protected List<Positioned<Output>> OutputOffsets = new List<Positioned<Output>>();

        protected Point GetCurrentSize()
        {
            return PreviousSize;
        }

        protected void SetCurrentSize(Point size)
        {
            PreviousSize = size;
            OnResize(PreviousSize.X, PreviousSize.Y);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            HasUpdatedIOPosThisRender = false;
            ElemWH elemWH = await JS.InvokeAsync<ElemWH>("JSUtils.getElementSize", SizeWatcher);
            Point newSize = new Point((int)elemWH.Width, (int)elemWH.Height);

            if (firstRender)
            {
                OnAfterFirstRenderAsync(newSize.X, newSize.Y);
            }

            bool sizeChanged = PreviousSize != newSize;
            PreviousSize = newSize;
            if (sizeChanged)
            {
                OnResize(newSize.X, newSize.Y);
            }

            bool hasMoved = PreviousPos != Position;
            PreviousPos = Position;
            if (hasMoved)
            {
                OnMove(Position);
            }
        }

        protected virtual void OnAfterFirstRenderAsync(int width, int height)
        { }

        protected virtual void OnResize(int width, int height)
        {
            OnComponentResized.InvokeAsync(new SizeUpdate(Operation, new Point(width, height)));

            InputOffsets = OnMakeInputs(width, height);
            OutputOffsets = OnMakeOutputs(width, height);

            if (!HasUpdatedIOPosThisRender)
            {
                HasUpdatedIOPosThisRender = true;

                List<Positioned<Input>> inputPoses = ToAbsolutePositions(InputOffsets);
                List<Positioned<Output>> outputPoses = ToAbsolutePositions(OutputOffsets);

                OnIOChangedPosition.InvokeAsync(new IOPositionUpdate(Operation, inputPoses, outputPoses));
            }
        }

        protected virtual void OnMove(Point newPos)
        {
            if (!HasUpdatedIOPosThisRender)
            {
                HasUpdatedIOPosThisRender = true;

                List<Positioned<Input>> inputPoses = ToAbsolutePositions(InputOffsets);
                List<Positioned<Output>> outputPoses = ToAbsolutePositions(OutputOffsets);

                OnIOChangedPosition.InvokeAsync(new IOPositionUpdate(Operation, inputPoses, outputPoses));
            }
        }

        protected List<Positioned<U>> ToAbsolutePositions<U>(List<Positioned<U>> positions)
        {
            List<Positioned<U>> absoPoses = new List<Positioned<U>>(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                Positioned<U> value = positions[i];
                absoPoses.Add(new Positioned<U>(value.Position + Position, value.Value));
            }

            return absoPoses;
        }

        protected abstract List<Positioned<Input>> OnMakeInputs(int width, int height);
        protected abstract List<Positioned<Output>> OnMakeOutputs(int width, int height);
    }
}
