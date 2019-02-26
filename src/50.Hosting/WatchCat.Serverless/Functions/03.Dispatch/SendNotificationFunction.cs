using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WatchCat.Connectors.Slack;
using WatchCat.Core;
using WatchCat.Serverless.DI;

namespace WatchCat.Serverless.Functions
{
    public static class SendNotificationFunction
    {
        [FunctionName("SendNotification")]
        public static async Task SendNotification(
            [ActivityTrigger] Notification notification,
            [OrchestrationClient] DurableOrchestrationClient client,
            ILogger log)
        {
            log.LogInformation($"Sending notification {notification.Id}.");
            
            try
            {
                var webHookClient = ServiceProvider.Resolve<ISlackWebHookClient>();
                await webHookClient.PublishMessageAsync(notification);
            }
            catch (Exception ex)
            {
                var jsonNotification = JsonConvert.SerializeObject(notification);
                log.LogError(ex, $"Couldn't publish notification {notification.Id} to slack. The following notification will be lost: {jsonNotification}.");
            }
        }
    }
}