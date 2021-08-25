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

WebHost.CreateDefaultBuilder()
    .ConfigureServices(s =>
    {
        s.AddHostedService<ScheduledMarketDataGet>();
        s.AddHttpClient("ScheduledMarketDataGet", hc =>
        {
            hc.BaseAddress = new System.Uri("https://api.binance.com/api/v3/ticker/price");
        });
        s.AddSingleton<MarketService>();
        s.AddSingleton(new InfluxDbClient("http://influxdb:8086/", "admin", "Welcome#123456", InfluxDbVersion.v_1_3));
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
