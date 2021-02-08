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
    public class FIRBase : ComponentBase
    {
        [Inject]
        private IJSRuntime JS { get; set; }

        [Parameter]
        public Point Position { get; set; }

        [Parameter]
        public EventCallback<Rectangle> OnComponentResized { get; set; }
        [Parameter]
        public EventCallback<IOPositions> OnIOChangedPosition { get; set; }

        private Point MainBodySize = new Point();
        protected ElementReference MainBody;
        protected List<Positioned<Input>> InputPoses = new List<Positioned<Input>>();
        protected List<Positioned<Output>> OutputPoses = new List<Positioned<Output>>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            ElemWH elemWH = await JS.InvokeAsync<ElemWH>("JSUtils.getElementSize", MainBody);
            Point size = new Point((int)elemWH.Width, (int)elemWH.Height);

            if (firstRender)
            {
                OnAfterFirstRenderAsync(size);
            }
            else
            {
                //if resized then remake IO
            }

            OnAfterFirstRenderAsync(size);

            //return base.OnAfterRenderAsync(firstRender);
        }

        protected virtual void OnAfterFirstRenderAsync(Point size)
        {
            OnResize(size.X, size.Y);
        }

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
