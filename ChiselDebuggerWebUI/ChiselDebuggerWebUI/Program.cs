using ChiselDebuggerRazor.Code;
using ChiselDebuggerWebUI.Code;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChiselDebuggerWebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IExampleCircuits, ExampleCircuits>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        //.ConfigureLogging(logging =>
        //{
        //    logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
        //    logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
        //});
    }
}
