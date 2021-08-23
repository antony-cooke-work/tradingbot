using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Market;

WebHost.CreateDefaultBuilder()
    .ConfigureServices(s => { 
        s.AddHostedService<ScheduledMarketDataGet>();
        s.AddHttpClient("ScheduledMarketDataGet", hc =>
        {
            hc.BaseAddress = new System.Uri("https://api.binance.com/api/v3/ticker/price");
        });
    })
    .Configure(app => app.Run(c => c.Response.WriteAsync("Market")))
    .Build()
    .Run();
