using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WatchCat.Connectors.SignalR;

namespace WatchCat.Adapters.SignalR
{
    public class SignalRAdapter :
        INotificationHandler<PayloadNotification>,
        INotificationHandler<MessageNotification>
    {
        private readonly SignalRConnector _connector;

        public SignalRAdapter(SignalRConnector connector)
        {
            _connector = connector;
        }

        Task INotificationHandler<PayloadNotification>.Handle(PayloadNotification notification, CancellationToken cancellationToken)
        {
            return _connector.SendAsync(notification);
        }

        Task INotificationHandler<MessageNotification>.Handle(MessageNotification notification, CancellationToken cancellationToken)
        {
            return _connector.SendAsync(notification);
        }
    }
}
