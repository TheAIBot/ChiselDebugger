using ChiselDebuggerRazor.Code.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace ChiselDebuggerRazor.Code
{
    public static class ChiselDebuggerServices
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddSingleton<WorkLimiter>();
            services.AddSingleton<DebugControllerFactory>();
            services.AddScoped<PageWideEvents>();
            services.AddScoped<IOWindowEvents>();
        }
    }
}
