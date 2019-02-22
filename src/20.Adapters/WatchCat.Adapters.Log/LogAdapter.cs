using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatchCat.Adapters.Log
{
    public class LogAdapter : INotificationHandler<LogNotification>
    {
        private readonly ILogger _logger;

        public LogAdapter(ILogger<LogAdapter> logger)
        {
            _logger = logger;
        }

        Task INotificationHandler<LogNotification>.Handle(LogNotification notification, CancellationToken cancellationToken)
        {
            _logger.Log(notification.Level, notification.Message);
            return Task.CompletedTask;
        }
    }
}
