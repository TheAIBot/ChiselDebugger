using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Controllers
{
    public sealed class CircuitLayout : FIRLayout
    {
        public override Task UpdateComponentInfoAsync(FIRComponentUpdate updateData)
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
