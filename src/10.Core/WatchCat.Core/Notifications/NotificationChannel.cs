using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchCat.Core.Notifications
{
    public abstract class NotificationChannel : INotificationChannel
    {
        public abstract string Description { get; }

        public bool SupportsNotificationType(Type notificationType) => true;

        public abstract Task NotifyAsync(Notification notification);
    }

    public abstract class NotificationChannel<TNotification>
        : INotificationChannel, INotificationChannel<TNotification>
        where TNotification : Notification
    {
        public abstract string Description { get; }

        bool INotificationChannel.SupportsNotificationType(Type notificationType) =>
            typeof(TNotification).IsAssignableFrom(notificationType);

        Task INotificationChannel.NotifyAsync(Notification notification)
        {
            var typedNotification = (TNotification)notification; // Explicit cast
            return this.NotifyAsync(typedNotification);
        }

        public abstract Task NotifyAsync(TNotification notification);
    }
}
