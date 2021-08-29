using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Market
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
                    s.AddHostedService<ScheduledMarketDataGet>();
                    s.AddHttpClient("ScheduledMarketDataGet", hc =>
                    {
                        hc.BaseAddress = new System.Uri("https://api.binance.com/api/v3/ticker/price");
                    });
                    s.AddSingleton<MarketService>();
                })
                .Configure(
                    app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(e =>
                        {
                            var service = e.ServiceProvider.GetRequiredService<MarketService>();
                            e.MapGet("/markets/{id}/{fromdatetime?}/{todatetime?}",
                                async s => await s.Response.WriteAsJsonAsync(await service.Get((string)s.Request.RouteValues["id"], (string)s.Request.RouteValues["fromdatetime"], (string)s.Request.RouteValues["todatetime"])));
                        });
                    })
                .Build()
                .Run();
        }
    }
}