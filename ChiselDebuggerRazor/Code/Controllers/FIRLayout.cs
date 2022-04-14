using ChiselDebuggerRazor.Components;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Controllers
{
    public abstract class FIRLayout
    {
        protected readonly ConcurrentBag<IFIRUINode> UINodes = new ConcurrentBag<IFIRUINode>();
        protected readonly ConcurrentBag<FIRLayout> ChildLayouts = new ConcurrentBag<FIRLayout>();

        public void AddUINode(IFIRUINode uiNode)
        {
            UINodes.Add(uiNode);
        }

        public void AddChildLayout(FIRLayout layout)
        {
            ChildLayouts.Add(layout);
        }

        public virtual void PrepareToRerenderLayout()
        {
            foreach (var uiNode in UINodes)
            {
                uiNode.PrepareForRender();
            }
        }

        public abstract Task UpdateComponentInfo(FIRComponentUpdate updateData);
        public abstract void UpdateLayoutDisplay(float scaling);
    }
}
