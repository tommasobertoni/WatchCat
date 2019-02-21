using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchCat.Core.Extensions.DependencyInjection;
using WatchCat.Core.Notifications;

namespace WatchCat.Core.Extensions.Notifications
{
    internal class NotificationDispatcher : INotificationDispatcher
    {
        private readonly Lazy<HandledPayloadTypesMap> _lazyHandledPayloadTypesMap;
        private HandledPayloadTypesMap HandledPayloadTypesMap => _lazyHandledPayloadTypesMap.Value;

        public NotificationDispatcher(IServiceProvider serviceProvider, NotificationDispatcherBuilder builder)
        {
            _lazyHandledPayloadTypesMap = new Lazy<HandledPayloadTypesMap>(() =>
            {
                var map = new HandledPayloadTypesMap(serviceProvider);

                builder.RegisteredNotificationChannelTypes.ToList()
                    .ForEach(t => LoadNotificationChannelType(t, map));

                return map;
            });

            // Local functions

            void LoadNotificationChannelType(Type notificationChannelType, HandledPayloadTypesMap map)
            {
                var handledPayloadTypes = notificationChannelType
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationChannel<>))
                    .Select(i => i.GetGenericArguments()[0])
                    .ToList();

                if (!handledPayloadTypes.Any()) return;

                foreach (var handledPayloadType in handledPayloadTypes)
                {
                    if (!map.ContainsKey(handledPayloadType))
                        map[handledPayloadType] = new List<Type>();

                    if (!map[handledPayloadType].Contains(notificationChannelType))
                        map[handledPayloadType].Add(notificationChannelType);
                }
            }
        }

        public async Task DispatchAsync<TPayload>(Notification<TPayload> notification)
        {
            if (notification == null) return;

            var notificationChannels = this.HandledPayloadTypesMap.GetNotificationChannels<TPayload>();
            if (notificationChannels?.Any() != true) return;

            await Task.WhenAll(notificationChannels.Select(ch => ch.HandleAsync(notification))).ConfigureAwait(false);
        }
    }

    internal class HandledPayloadTypesMap : Dictionary<Type, List<Type>>
    {
        private readonly IServiceProvider _serviceProvider;

        public HandledPayloadTypesMap(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public List<INotificationChannel<TPayload>> GetNotificationChannels<TPayload>()
        {
            var payloadType = typeof(TPayload);

            if (this.TryGetValue(payloadType, out var channels))
            {
                return channels
                    .Select(t => _serviceProvider.GetService(t) as INotificationChannel<TPayload>)
                    .Where(channel => channel != null)
                    .ToList();
            }

            return new List<INotificationChannel<TPayload>>();
        }
    }
}
