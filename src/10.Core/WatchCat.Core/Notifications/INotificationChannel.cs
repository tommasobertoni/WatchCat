using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchCat.Core.Notifications
{
    public interface INotificationChannel
    {
        string Description { get; }

        bool SupportsNotificationType(Type notificationType);

        Task NotifyAsync(Notification notification);
    }

    public interface INotificationChannel<TNotification> : INotificationChannel
        where TNotification : Notification
    {
        Task NotifyAsync(TNotification notification);
    }
}
