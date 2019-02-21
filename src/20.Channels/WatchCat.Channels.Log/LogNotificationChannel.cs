using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchCat.Core.Notifications;

namespace WatchCat.Channels.Log
{
    public class LogMulti : INotificationChannel, INotificationChannel<string>
    {
        private readonly ILogger _logger;

        public LogMulti(ILogger<LogMulti> logger)
        {
            _logger = logger;
        }

        Task INotificationChannel<object>.HandleAsync(Notification<object> notification)
        {
            string message = $"[{notification.CreatedAtUtc}] {notification.Id}: LOG MULTI (object) {notification.Payload}";
            _logger?.LogInformation(message);
            return Task.CompletedTask;
        }

        Task INotificationChannel<string>.HandleAsync(Notification<string> notification)
        {
            string message = $"[{notification.CreatedAtUtc}] {notification.Id}: LOG MULTI (string) {notification.Payload}";
            _logger?.LogInformation(message);
            return Task.CompletedTask;
        }
    }

    public class LogNotificationChannel : INotificationChannel
    {
        private readonly ILogger _logger;

        public LogNotificationChannel(ILogger<LogNotificationChannel> logger)
        {
            _logger = logger;
        }

        Task INotificationChannel<object>.HandleAsync(Notification<object> notification)
        {
            string message = $"[{notification.CreatedAtUtc}] {notification.Id}: (object) {notification.Payload}";
            _logger?.LogInformation(message);
            return Task.CompletedTask;
        }
    }

    public class LogStringsNotificationChannel : INotificationChannel<string>
    {
        private readonly ILogger _logger;

        public LogStringsNotificationChannel(ILogger<LogNotificationChannel> logger)
        {
            _logger = logger;
        }

        Task INotificationChannel<string>.HandleAsync(Notification<string> notification)
        {
            string message = $"[{notification.CreatedAtUtc}] {notification.Id}: (string) {notification.Payload}";
            _logger?.LogInformation(message);
            return Task.CompletedTask;
        }
    }
}
