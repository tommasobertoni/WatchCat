using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WatchCat.Serverless.Components;
using WatchCat.Serverless.DI;
using WatchCat.Serverless.Orchestration;

namespace WatchCat.Serverless.Functions
{
    public static class ResetAggregatorFunction
    {
        [FunctionName("ResetAggregator")]
        public static async Task ResetAggregator(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [OrchestrationClient] DurableOrchestrationClient client,
            ILogger log)
        {
            var config = ServiceProvider.Resolve<Configuration>();
            var status = await client.GetStatusAsync(config.AggregatorFunctionId);

            if (status != null)
            {
                await client.PurgeInstanceHistoryAsync(config.AggregatorFunctionId);

                var factory = ServiceProvider.Resolve<OrchestratorFactory>();

                // Ensure that aggregator is running
                await factory.TryStartNewAsync(client,
                    nameof(AggregatorFunction.Aggregator),
                    config.AggregatorFunctionId,
                    null, startIfAlreadyCompleted: true);
            }
        }
    }
}
