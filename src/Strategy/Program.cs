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
                .ConfigureServices((hostingContext, services) =>
                {
                    var MARKET_API_URI = hostingContext.Configuration.GetValue<string>("MARKET_API_URI");
                    services.AddHttpClient("StrategyService", hc =>
                    {
                        hc.BaseAddress = new System.Uri(MARKET_API_URI);
                    });
                    services.AddSingleton<StrategyService>();
                })
                .Configure(
                    app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(e =>
                        {
                            var service = e.ServiceProvider.GetRequiredService<StrategyService>();
                            e.MapGet("/smai/{id}/{start}/{stop}/{shorttermevery}/{shorttermperiod}/{longtermevery}/{longtermperiod}",
                                async s => await s.Response.WriteAsJsonAsync(
                                    await service.GetMovingAverageIndicatorAsync(
                                        (string)s.Request.RouteValues["id"], 
                                        (string)s.Request.RouteValues["start"], 
                                        (string)s.Request.RouteValues["stop"], 
                                        (string)s.Request.RouteValues["shorttermevery"], 
                                        (string)s.Request.RouteValues["shorttermperiod"], 
                                        (string)s.Request.RouteValues["longtermevery"], 
                                        (string)s.Request.RouteValues["longtermperiod"])));
                        });
                    })
                .Build()
                .Run();
        }
    }
}