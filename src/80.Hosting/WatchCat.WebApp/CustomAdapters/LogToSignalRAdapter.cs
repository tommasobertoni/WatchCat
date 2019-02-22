using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WatchCat.Adapters.Log;
using WatchCat.Adapters.SignalR;
using WatchCat.Connectors.SignalR;

namespace WatchCat.WebApp.Combinators
{
    public class LogToSignalRAdapter : INotificationHandler<LogNotification>
    {
        private readonly SignalRConnector _connector;

        public LogToSignalRAdapter(SignalRConnector connector)
        {
            _connector = connector;
        }

        Task INotificationHandler<LogNotification>.Handle(LogNotification notification, CancellationToken cancellationToken)
        {
            return _connector.SendAsync(notification);
        }
    }
}
