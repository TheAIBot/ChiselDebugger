using ChiselDebuggerWebUI.Components;
using System.Collections.Generic;

namespace ChiselDebuggerWebUI.Code
{
    public abstract class FIRLayout
    {
        protected readonly List<IFIRUINode> UINodes = new List<IFIRUINode>();

        public void AddUINode(IFIRUINode uiNode)
        {
            UINodes.Add(uiNode);
        }

        public void PrepareToRerenderLayout()
        {
            foreach (var uiNode in UINodes)
            {
                uiNode.PrepareForRender();
            }
        }

        public abstract void UpdateComponentInfo(FIRComponentUpdate updateData);
    }
}
