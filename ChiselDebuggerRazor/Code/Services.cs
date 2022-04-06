using ChiselDebuggerRazor.Code.Templates;
using Microsoft.Extensions.DependencyInjection;

namespace ChiselDebuggerRazor.Code
{
    public static class ChiselDebuggerServices
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddTransient<PlacementTemplator>();
            services.AddTransient<RouteTemplator>();
            services.AddSingleton<WorkLimiter>();
        }
    }
}
