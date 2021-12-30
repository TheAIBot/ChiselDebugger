using Microsoft.AspNetCore.Hosting;
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
