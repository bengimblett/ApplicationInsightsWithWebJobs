﻿namespace AppInsightsWithWebJob
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder();

            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
            });

            builder.ConfigureLogging((context, b) =>
            {
                b.SetMinimumLevel(LogLevel.Debug);
                b.AddConsole();
                b.AddApplicationInsightsWebJobs(options =>
                {
                    var instrumentationKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                    Console.WriteLine($"Instrumentation key: {instrumentationKey}");
                    options.InstrumentationKey = instrumentationKey;
                });
            });

            builder.ConfigureServices((context, services) =>
            {
                //services.AddSingleton<ITelemetryInitializer, MyTelemetryInitializer>();
                services.AddSingleton<IHostedService, ContinuousJob>();
            });

            await builder.Build().RunAsync();
        }
    }
}
