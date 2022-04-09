﻿using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Controllers
{
    public class CircuitLayout : FIRLayout
    {
        public override Task UpdateComponentInfo(FIRComponentUpdate updateData)
        { 
            return Task.CompletedTask;
        }

        public override void UpdateLayoutDisplay(float scaling)
        {
            foreach (var childLayout in ChildLayouts)
            {
                childLayout.UpdateLayoutDisplay(scaling);
            }
        }
    }
}
