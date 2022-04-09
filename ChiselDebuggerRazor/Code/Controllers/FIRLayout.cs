using ChiselDebuggerRazor.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Controllers
{
    public abstract class FIRLayout
    {
        protected readonly List<IFIRUINode> UINodes = new List<IFIRUINode>();
        protected readonly List<FIRLayout> ChildLayouts = new List<FIRLayout>();

        public void AddUINode(IFIRUINode uiNode)
        {
            lock (UINodes)
            {
                UINodes.Add(uiNode);
            }
        }

        public void AddChildLayout(FIRLayout layout)
        {
            lock (ChildLayouts)
            {
                ChildLayouts.Add(layout);
            }
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
