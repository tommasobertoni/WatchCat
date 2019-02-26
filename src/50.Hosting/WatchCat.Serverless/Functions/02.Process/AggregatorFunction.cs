using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using WatchCat.Core;
using WatchCat.Serverless.Components;
using WatchCat.Serverless.DI;

namespace WatchCat.Serverless.Functions
{
    public static class AggregatorFunction
    {
        [FunctionName("Aggregator")]
        public static async Task Aggregator(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger log)
        {
            if (!context.IsReplaying) log.LogInformation("Aggregator function started.");

            var config = ServiceProvider.Resolve<Configuration>();

            var requestedEventsCount = config.EventsMaxBatchSize;
            
            var messages = await context.CallActivityAsync<List<Message>>(
                nameof(FetchNextQueueMessages), requestedEventsCount);

            if (messages.Any())
            {
                var notification = await context.CallActivityAsync<Notification>(nameof(CreateNotification), messages);
                await context.CallActivityAsync(nameof(SendNotificationFunction.SendNotification), notification);

                // Delete processed messages
                await context.CallActivityAsync(nameof(DeleteMessages), messages);

                if (!context.IsReplaying) log.LogInformation($"Aggregator processed {messages.Count} messages: continue as new.");

                if (messages.Count >= requestedEventsCount)
                    // Continue until there are no more messages in the queue.
                    context.ContinueAsNew(null);
            }

            if (!context.IsReplaying) log.LogInformation($"Aggregator stopping because there are no more messages to process.");
        }

        [FunctionName("FetchNextQueueMessages")]
        public static async Task<List<Message>> FetchNextQueueMessages(
            [ActivityTrigger] int messagesCount,
            ILogger log)
        {
            var storage = ServiceProvider.Resolve<Storage>();
            var queue = await storage.GetBufferQueueAsync();

            try
            {
                // Must return a json serializable type!
                var allFetchedMessages = new List<Message>();

                while (allFetchedMessages.Count < messagesCount)
                {
                    // The number of messages to retrieve. The maximum number of messages that may be retrieved at one time is 32.
                    int messagesToRetrieve = Math.Min(messagesCount - allFetchedMessages.Count, 32);
                    var queueMessages = (await queue.GetMessagesAsync(
                        messagesToRetrieve, TimeSpan.FromMinutes(1), null, null))
                        .ToList();

                    if (queueMessages.Any())
                    {
                        var messages = queueMessages.Select(m => new Message
                        {
                            Id = m.Id,
                            Content = m.AsString,
                            PopReceipt = m.PopReceipt,
                            InsertionTime = m.InsertionTime?.DateTime.ToUniversalTime()
                        }).OrderBy(m => m.InsertionTime);

                        allFetchedMessages.AddRange(messages);
                    }

                    if (queueMessages.Count < messagesToRetrieve)
                        break; // There are no more messages in the queue.
                }

                return allFetchedMessages;
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Couldn't fetch queue messages, messages count: {messagesCount}.");
                return new List<Message>();
            }
        }

        [FunctionName("CreateNotification")]
        public static Notification CreateNotification(
            [ActivityTrigger] List<Message> messages,
            ILogger log)
        {
            var formattedMessages = messages
                .Select(m => m.Content)
                .Select(x => x.Length > 500 ? $"{x.Substring(0, 500)}..." : x)
                .Select(x => $"\r\n- {x}")
                .ToList();

            var items = messages
                .Select(m => new NotificationItem(m.Content, m.Id))
                .ToList();

            var messagesStrings = string.Join(string.Empty, formattedMessages);

            var notification = new Notification(
                message: $"{messages.Count} needs attention:{messagesStrings}",
                items: items);

            return notification;
        }

        [FunctionName("DeleteMessages")]
        public static async Task DeleteMessages(
            [ActivityTrigger] List<Message> messages,
            ILogger log)
        {
            var storage = ServiceProvider.Resolve<Storage>();
            var queue = await storage.GetBufferQueueAsync();

            await Task.WhenAll(messages.Select(async m =>
            {
                try
                {
                    await queue.DeleteMessageAsync(m.Id, m.PopReceipt);
                }
                catch (Exception ex)
                {
                    log.LogError(ex, $"Couldn't delete message {m.Id}:{m.PopReceipt} from queue.");
                }
            }));
        }
    }

    public class Message
    {
        public string Id { get; set; }

        public string Content { get; set; }

        public string PopReceipt { get; set; }

        public DateTime? InsertionTime { get; set; }
    }
}