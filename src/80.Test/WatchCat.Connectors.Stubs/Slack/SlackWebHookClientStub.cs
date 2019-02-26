using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchCat.Connectors.Slack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WatchCat.Core;

namespace WatchCat.Connectors.Stubs.Slack
{
    public class SlackWebHookClientStub : ISlackWebHookClient
    {
        private const string Separator = "\n-------------------------------\n";

        private readonly ILogger _logger;

        public SlackWebHookClientStub(ILogger logger)
        {
            _logger = logger;
        }

        public Task PublishMessageAsync(Notification notification)
        {
            var json = JsonConvert.SerializeObject(notification, Formatting.Indented);
            var logMessage = $"{Separator}Publish message:\n{json}{Separator}";
            _logger.LogInformation(logMessage);

            return Task.CompletedTask;
        }
    }
}
