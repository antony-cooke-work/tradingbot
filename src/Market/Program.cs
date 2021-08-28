using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Market;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.Common.Enums;
using Microsoft.Extensions.Configuration;

WebHost.CreateDefaultBuilder()
    .ConfigureAppConfiguration((hostingContext, configuration) =>
    {
        configuration.Sources.Clear();

        IHostEnvironment env = hostingContext.HostingEnvironment;

        configuration
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json")
            .AddEnvironmentVariables()
            .Build();

        // IConfigurationRoot configurationRoot = configuration.Build();
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
                e.MapGet("/markets/{id}",
                    async s => await s.Response.WriteAsJsonAsync(await service.Get((string)s.Request.RouteValues["id"])));
            });
        })
    .Build()
    .Run();
