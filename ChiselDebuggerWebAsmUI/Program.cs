using ChiselDebug.Placing;
using ChiselDebuggerRazor.Code;
using ChiselDebuggerWebAsmUI.Code;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChiselDebuggerWebAsmUI
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            ChiselDebuggerServices.AddServices(builder.Services);
            builder.Services.AddSingleton<IWorkLimiter, WorkLimiter>();
            builder.Services.AddSingleton<INodePlacerFactory, SimplePlacerFactory>();

            return builder.Build().RunAsync();
        }
    }
}
