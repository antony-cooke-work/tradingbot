using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Market
{
    public class Program
    {
        public IConfiguration Configuration { get; }

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
                    services.AddHostedService<ScheduledMarketDataGet>();
                    var MARKET_EXCHANGE_URI = hostingContext.Configuration.GetValue<string>("EXCHANGE_URI");
                    services.AddHttpClient("ScheduledMarketDataGet", hc =>
                    {
                        hc.BaseAddress = new System.Uri(MARKET_EXCHANGE_URI);
                    });
                    services.AddSingleton<MarketService>();
                })
                .Configure(
                    app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(e =>
                        {
                            var service = e.ServiceProvider.GetRequiredService<MarketService>();

                            e.MapGet("/prices/{id}/{fromdatetime?}/{todatetime?}",
                                    async s => await s.Response.WriteAsJsonAsync(
                                        await service.Get(
                                            (string)s.Request.RouteValues["id"], 
                                            (string)s.Request.RouteValues["fromdatetime"], 
                                            (string)s.Request.RouteValues["todatetime"])));

                            e.MapGet("/smas/{id}/{start?}/{stop?}/{every?}/{period?}",
                                    async s => await s.Response.WriteAsJsonAsync(
                                        await service.SimpleMovingAverages(
                                            (string)s.Request.RouteValues["id"], 
                                            (string)s.Request.RouteValues["start"],
                                            (string)s.Request.RouteValues["stop"],
                                            (string)s.Request.RouteValues["every"], 
                                            (string)s.Request.RouteValues["period"])));
                            });
                        })
                .Build()
                .Run();
        }
    }
}