namespace ChiselDebuggerWebUI.Code
{
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
