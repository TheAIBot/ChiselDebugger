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
        protected List<Positioned<Input>> InputPoses = new List<Positioned<Input>>();
        protected List<Positioned<Output>> OutputPoses = new List<Positioned<Output>>();

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
            }

            PreviousSize = newSize;
        }

        protected virtual void OnAfterFirstRenderAsync(int width, int height)
        { }

        protected virtual void OnResize(int width, int height)
        {
            Point size = new Point(width, height);
            List<Positioned<Input>> inputOffsets = OnMakeInputs(size.X, size.Y);
            List<Positioned<Output>> outputOffsets = OnMakeOutputs(size.X, size.Y);

            //Convert relative positions to absolute positions
            for (int i = 0; i < inputOffsets.Count; i++)
            {
                inputOffsets[i].Position += Position;
            }
            for (int i = 0; i < outputOffsets.Count; i++)
            {
                outputOffsets[i].Position += Position;
            }

            InputPoses = inputOffsets;
            OutputPoses = outputOffsets;

            OnComponentResized.InvokeAsync(new Rectangle(Position, size.X, size.Y));
            OnIOChangedPosition.InvokeAsync(new IOPositions(InputPoses, OutputPoses));

            StateHasChanged();
        }

        protected virtual List<Positioned<Input>> OnMakeInputs(int width, int height)
        {
            return new List<Positioned<Input>>();
        }

        protected virtual List<Positioned<Output>> OnMakeOutputs(int width, int height)
        {
            return new List<Positioned<Output>>();
        }
    }
}
