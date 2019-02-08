using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchCat.Core.Extensions.DependencyInjection;
using WatchCat.Core.Notifications;

namespace WatchCat.Core.Extensions.Notifications
{
    public interface IBroadcastNotificationChannel : INotificationChannel
    {
    }

    internal class BroadcastNotificationChannel : IBroadcastNotificationChannel
    {
        private static readonly Type _BroadcastNotificationChannelType = typeof(BroadcastNotificationChannel);

        private readonly IServiceProvider _serviceProvider;
        private readonly NotificationChannelsBuilder _builder;
        private readonly Lazy<IEnumerable<INotificationChannel>> _lazyNotificationChannels;

        public string Description
        {
            get
            {
                var notificationChannels = _lazyNotificationChannels.Value;
                if (notificationChannels == null || !notificationChannels.Any()) return string.Empty;
                return string.Join(", ", notificationChannels.Select(c => c.Description));
            }
        }

        public BroadcastNotificationChannel(IServiceProvider serviceProvider, NotificationChannelsBuilder builder)
        {
            _serviceProvider = serviceProvider;
            _builder = builder;
            _lazyNotificationChannels = new Lazy<IEnumerable<INotificationChannel>>(() =>
            {
                return _builder
                    .RegisteredNotificationChannelTypes
                    .Where(type => type != _BroadcastNotificationChannelType)
                    .Select(type => (INotificationChannel)_serviceProvider.GetService(type));
            });
        }

        public bool SupportsNotificationType(Type notificationType) => true;

        public async Task NotifyAsync(Notification notification)
        {
            if (notification == null) return;

            var notificationChannels = _lazyNotificationChannels.Value;
            if (notificationChannels == null || !notificationChannels.Any()) return;

            var notificationType = notification.GetType();

            var notificationTasks = notificationChannels.Select(channel =>
            {
                return channel.SupportsNotificationType(notificationType)
                    ? channel.NotifyAsync(notification)
                    : Task.CompletedTask;
            });

            await Task.WhenAll(notificationTasks);
        }
    }
}
