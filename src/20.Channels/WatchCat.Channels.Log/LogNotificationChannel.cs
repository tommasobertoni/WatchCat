using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchCat.Core.Notifications;

namespace WatchCat.Channels.Log
{
    public class LogNotificationChannel : NotificationChannel
    {
        public override string Description => "Log";

        private readonly ILogger _logger;

        public LogNotificationChannel(ILogger<LogNotificationChannel> logger)
        {
            _logger = logger;
        }

        public override Task NotifyAsync(Notification notification)
        {
            string message = notification switch
            {
                null => null,
                MessageNotification mn => $"{nameof(MessageNotification)} {notification.Id}: {mn.Message}",
                _ => $"{nameof(MessageNotification)} {notification.Id}"
            };

            if (message != null)
                _logger?.LogInformation(message);

            return Task.CompletedTask;
        }
    }
}
