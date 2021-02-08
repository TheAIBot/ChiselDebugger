using ChiselDebug;
using ChiselDebuggerWebUI.Code;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Components
{
    public abstract class FIRBase : ComponentBase
    {
        [Inject]
        private IJSRuntime JS { get; set; }

        [Parameter]
        public Point Position { get; set; }

        [Parameter]
        public EventCallback<Rectangle> OnComponentResized { get; set; }
        [Parameter]
        public EventCallback<IOPositions> OnIOChangedPosition { get; set; }

        private Point PreviousSize = new Point(0, 0);
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
            ElemWH elemWH = await JS.InvokeAsync<ElemWH>("JSUtils.getElementSize", SizeWatcher);
            Point newSize = new Point((int)elemWH.Width, (int)elemWH.Height);

            if (firstRender)
            {
                OnAfterFirstRenderAsync(newSize.X, newSize.Y);
            }

            if (PreviousSize != newSize)
            {
                OnResize(newSize.X, newSize.Y);
                StateHasChanged();
            }

            PreviousSize = newSize;
        }

        protected virtual void OnAfterFirstRenderAsync(int width, int height)
        { }

        protected virtual void OnResize(int width, int height)
        {
            InputOffsets = OnMakeInputs(width, height);
            OutputOffsets = OnMakeOutputs(width, height);

            List<Positioned<Input>> inputPoses = ToAbsolutePositions(InputOffsets);
            List<Positioned<Output>> outputPoses = ToAbsolutePositions(OutputOffsets);

            OnComponentResized.InvokeAsync(new Rectangle(Position, width, height));
            OnIOChangedPosition.InvokeAsync(new IOPositions(inputPoses, outputPoses));
        }

        protected List<Positioned<T>> ToAbsolutePositions<T>(List<Positioned<T>> positions)
        {
            List<Positioned<T>> absoPoses = new List<Positioned<T>>(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                Positioned<T> value = positions[i];
                absoPoses.Add(new Positioned<T>(value.Position + Position, value.Value));
            }

            return absoPoses;
        }

        protected abstract List<Positioned<Input>> OnMakeInputs(int width, int height);
        protected abstract List<Positioned<Output>> OnMakeOutputs(int width, int height);
    }
}
