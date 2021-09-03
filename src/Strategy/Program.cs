using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Strategy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();

                    IHostEnvironment env = hostingContext.HostingEnvironment;

                    configuration
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                        .AddEnvironmentVariables()
                        .Build();
                })
                .ConfigureServices(s =>
                {
                    //s.AddHttpClient("ScheduledStrategyDataGet", hc =>
                    //{
                    //    hc.BaseAddress = new System.Uri("https://api.binance.com/api/v3/ticker/price");
                    //});
                    s.AddSingleton<StrategyService>();
                })
                .Configure(
                    app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(e =>
                        {
                            var service = e.ServiceProvider.GetRequiredService<StrategyService>();
                            //e.MapGet("/strategys/{id}",
                            //    async s => await s.Response.WriteAsJsonAsync(await service.Get((string)s.Request.RouteValues["id"])));
                        });
                    })
                .Build()
                .Run();
        }
    }
}