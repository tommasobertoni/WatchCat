using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WatchCat.Serverless.DI;
using WatchCat.Serverless.Components;
using WatchCat.Core.Events;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace WatchCat.Serverless.Functions
{
    public static class GateFunction
    {
        [FunctionName("Gate")]
        public static async Task<IActionResult> Gate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [OrchestrationClient] DurableOrchestrationClient client,
            ILogger log)
        {
            EventEntry @event = null;

            try
            {
                var extractor = ServiceProvider.Resolve<EventsExtractor>();
                @event = await extractor.FromHttpRequest(req);
            } catch (Exception ex)
            {
                string message = "Couldn't extract event from http request.";
                log.LogError(ex, message);
                return new BadRequestObjectResult(message);
            }

            // TODO: check blacklist

            try
            {
                var storage = ServiceProvider.Resolve<Storage>();
                var json = JsonConvert.SerializeObject(@event.Content);
                var queue = await storage.GetBufferQueueAsync();
                await queue.AddMessageAsync(new CloudQueueMessage(json));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Couldn't add parsed event to the buffer queue.");
                throw;
            }

            return new NoContentResult();
        }
    }
}
