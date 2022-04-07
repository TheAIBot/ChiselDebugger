using ChiselDebuggerRazor.Code.Controllers;
using ChiselDebuggerRazor.Code.Templates;
using Microsoft.Extensions.DependencyInjection;

namespace ChiselDebuggerRazor.Code
{
    public static class ChiselDebuggerServices
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddSingleton<WorkLimiter>();
            services.AddSingleton<DebugControllerFactory>();
        }
    }
}
