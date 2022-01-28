namespace AppInsightsWithWebJob
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using Microsoft.ApplicationInsights;

    public class ContinuousJob : BackgroundService
    {
        readonly ILogger<ContinuousJob> logger;
        readonly TelemetryClient telemetryClient;

        readonly string roleInstanceId;

        public ContinuousJob(ILogger<ContinuousJob> logger, TelemetryClient telemetryClient)
        {
            this.logger = logger;
            this.telemetryClient = telemetryClient;
            this.roleInstanceId = System.Environment.GetEnvironmentVariable("RoleInstanceId");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"{roleInstanceId} executing WebJob through BackgroundService() => ExecuteAsync()");


            try
            {
                var counter = 0;
                int msDelay = 500;
  
                while (!stoppingToken.IsCancellationRequested)
                {
                    try{
                        await Task.Delay(msDelay, stoppingToken);
                    }
                    catch (TaskCanceledException){
                        logger.LogWarning($"{roleInstanceId} >> Cancel requested in child task, need to clean up here now <<");
                    }

                    if ( counter % 5 == 0)
                        logger.LogDebug($"{roleInstanceId} Counter is at {counter}", counter);

                    counter++;
                }
                if ( stoppingToken.IsCancellationRequested ){
                    logger.LogWarning($"{roleInstanceId} >> Cancel requested, Exited work loop, Cleaning up now, ExecuteAsync() exits <<");
                }
            }

            catch (Exception exception)
            {
                logger.LogCritical($"{roleInstanceId} ExecuteAsync() Continuous job threw an exceptions. {0}", exception);
                telemetryClient.TrackException(exception);
            }
        }

    }
}