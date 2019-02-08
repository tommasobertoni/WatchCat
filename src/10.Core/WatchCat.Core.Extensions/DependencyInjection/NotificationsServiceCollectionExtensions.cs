using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchCat.Core.Extensions.Notifications;
using WatchCat.Core.Notifications;

namespace WatchCat.Core.Extensions.DependencyInjection
{
    public static class NotificationsServiceCollectionExtensions
    {
        private static readonly Type _BroadcastNotificationChannelType = typeof(BroadcastNotificationChannel);

        public static INotificationChannelsBuilder AddNotificationChannels(this IServiceCollection services)
        {
            var builder = new NotificationChannelsBuilder(services);
            return builder;
        }

        public static INotificationChannelsBuilder Register<TNotificationChannel>(this INotificationChannelsBuilder builder)
        {
            builder.TryRegister(typeof(TNotificationChannel));
            return builder;
        }

        public static INotificationChannelsBuilder WithAllAvailableNotificationChannels(this INotificationChannelsBuilder builder)
        {
            if (builder is NotificationChannelsBuilder concreteBuilder)
            {
                concreteBuilder.LoadNotificationChannels();
            }

            return builder;
        }

        public static INotificationChannelsBuilder WithBroadcastNotificationChannel(this INotificationChannelsBuilder builder)
        {
            if (builder is NotificationChannelsBuilder concreteBuilder &&
                !concreteBuilder.IsNotificationChannelTypeRegistered(_BroadcastNotificationChannelType))
            {
                concreteBuilder.Services.AddSingleton(sp => new BroadcastNotificationChannel(sp, concreteBuilder));
                concreteBuilder.Services.AddSingleton<INotificationChannel>(sp => sp.GetRequiredService<BroadcastNotificationChannel>());
                concreteBuilder.Services.AddSingleton<IBroadcastNotificationChannel>(sp => sp.GetRequiredService<BroadcastNotificationChannel>());
            }

            return builder;
        }
    }
}
