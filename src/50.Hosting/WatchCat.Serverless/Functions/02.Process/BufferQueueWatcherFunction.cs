using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WatchCat.Serverless.Components;
using WatchCat.Serverless.DI;
using WatchCat.Serverless.Orchestration;

namespace WatchCat.Serverless.Functions
{
    public static class BufferQueueWatcherFunction
    {
        [FunctionName("BufferQueueWatcher")]
        public static async Task BufferQueueWatcher(
            [TimerTrigger("*/5 * * * * *")]TimerInfo myTimer,
            [OrchestrationClient] DurableOrchestrationClient client,
            ILogger log)
        {
            var storage = ServiceProvider.Resolve<Storage>();
            var queue = await storage.GetBufferQueueAsync();

            await queue.FetchAttributesAsync();
            var approximateMessageCount = queue.ApproximateMessageCount.GetValueOrDefault();
#if TEST
            log.LogWarning($"Approximate messages count: {approximateMessageCount}");
#endif
            if (approximateMessageCount > 0)
            {
                var config = ServiceProvider.Resolve<Configuration>();
                var factory = ServiceProvider.Resolve<OrchestratorFactory>();

                // Ensure that aggregator is running
                bool startedNew = await factory.TryStartNewAsync(client,
                    nameof(AggregatorFunction.Aggregator),
                    config.AggregatorFunctionId,
                    null, startIfAlreadyCompleted: true);

                if (startedNew)
                    log.LogInformation($"Aggregator function restarted from {nameof(BufferQueueWatcher)}.");
            }
        }
    }
}
