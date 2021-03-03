using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Components
{
    public abstract class ManualRenderBase : ComponentBase
    {
        protected bool HasToRender = true;

        protected override bool ShouldRender()
        {
            bool doRender = HasToRender;
            HasToRender = false;
            return doRender;
        }

        protected new void StateHasChanged()
        {
            HasToRender = true;
            base.StateHasChanged();
        }
    }
}
