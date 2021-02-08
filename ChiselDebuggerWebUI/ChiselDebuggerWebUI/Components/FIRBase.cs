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
            InputOffsets = OnMakeInputs(width, height);
            OutputOffsets = OnMakeOutputs(width, height);

            List<Positioned<Input>> inputPoses = new List<Positioned<Input>>(InputOffsets.Count);
            List<Positioned<Output>> outputPoses = new List<Positioned<Output>>(OutputOffsets.Count);

            //Convert relative positions to absolute positions
            for (int i = 0; i < InputOffsets.Count; i++)
            {
                Positioned<Input> input = InputOffsets[i];
                inputPoses.Add(new Positioned<Input>(input.Position + Position, input.Value));
            }
            for (int i = 0; i < OutputOffsets.Count; i++)
            {
                Positioned<Output> output = OutputOffsets[i];
                outputPoses.Add(new Positioned<Output>(output.Position + Position, output.Value));
            }

            OnComponentResized.InvokeAsync(new Rectangle(Position, width, height));
            OnIOChangedPosition.InvokeAsync(new IOPositions(inputPoses, outputPoses));

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
