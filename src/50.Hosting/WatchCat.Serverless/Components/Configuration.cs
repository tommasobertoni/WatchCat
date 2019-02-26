using System;
using System.Collections.Generic;
using System.Text;

namespace WatchCat.Serverless.Components
{
    public class Configuration
    {
        public string AggregatorFunctionId { get; }

        public string EventsBufferQueueName { get; }

        public int EventsMaxBatchSize { get; }

        public string SlackChannelWebHook { get; }

        public Configuration()
        {
            this.AggregatorFunctionId =
                Environment.GetEnvironmentVariable(nameof(AggregatorFunctionId)) ??
                Guid.NewGuid().ToString();

            this.EventsBufferQueueName =
                Environment.GetEnvironmentVariable(nameof(EventsBufferQueueName)) ??
                throw new ArgumentNullException(nameof(EventsBufferQueueName));
            
            this.EventsMaxBatchSize =
                int.TryParse(Environment.GetEnvironmentVariable(nameof(EventsMaxBatchSize)), out var maxBatchSize)
                ? maxBatchSize : 100;

            this.SlackChannelWebHook =
                Environment.GetEnvironmentVariable(nameof(SlackChannelWebHook)) ??
                throw new ArgumentNullException(nameof(SlackChannelWebHook));
        }
    }
}
