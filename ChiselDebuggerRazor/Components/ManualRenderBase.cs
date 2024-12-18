using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Components
{
    public abstract class ManualRenderBase : ComponentBase
    {
        protected bool HasToRender = true;
        private bool IsFirstSetParametersEvent = true;

        protected override bool ShouldRender()
        {
            bool doRender = HasToRender;
            HasToRender = false;
            return doRender;
        }

        public new void StateHasChanged()
        {
            HasToRender = true;
            base.StateHasChanged();
        }

        public Task InvokeStateHasChangedAsync()
        {
            return InvokeAsync(StateHasChanged);
        }

        protected override Task OnParametersSetAsync()
        {
            if (IsFirstSetParametersEvent)
            {
                IsFirstSetParametersEvent = false;

                return OnFirstParametersSetAsync();
            }
            return Task.CompletedTask;
        }

        protected virtual Task OnFirstParametersSetAsync()
        {
            return Task.CompletedTask;
        }
    }
}
