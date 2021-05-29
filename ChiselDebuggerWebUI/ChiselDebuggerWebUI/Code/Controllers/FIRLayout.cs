using ChiselDebug;
using ChiselDebuggerWebUI.Components;
using System.Collections.Generic;

namespace ChiselDebuggerWebUI.Code
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

        public abstract void UpdateComponentInfo(FIRComponentUpdate updateData);
        public abstract void UpdateLayoutDisplay(float scaling);
    }

    public class CircuitLayout : FIRLayout
    {
        public override void UpdateComponentInfo(FIRComponentUpdate updateData)
        { }

        public override void UpdateLayoutDisplay(float scaling)
        {
            foreach (var childLayout in ChildLayouts)
            {
                childLayout.UpdateLayoutDisplay(scaling);
            }
        }
    }
}
