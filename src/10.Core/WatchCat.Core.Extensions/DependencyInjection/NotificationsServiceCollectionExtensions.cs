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
        public static INotificationDispatcherBuilder AddNotificationDispatcher(this IServiceCollection services)
        {
            var builder = new NotificationDispatcherBuilder();

            builder.OnNotificationChannelTypeRegistered += notificationChannelType =>
            {
                if (!services.Any(sd => sd.ServiceType == notificationChannelType))
                    services.AddScoped(notificationChannelType);
            };

            services.AddScoped(sp => new NotificationDispatcher(sp, builder));
            services.AddScoped<INotificationDispatcher>(sp => sp.GetRequiredService<NotificationDispatcher>());
            services.AddScoped<INotificationDispatcher>(sp => sp.GetRequiredService<NotificationDispatcher>());

            return builder;
        }

        public static INotificationDispatcherBuilder Register<TNotificationChannel, TPayload>(this INotificationDispatcherBuilder builder)
            where TNotificationChannel : INotificationChannel<TPayload>
        {
            builder.TryRegister(typeof(TNotificationChannel));
            return builder;
        }

        public static INotificationDispatcherBuilder Register<TNotificationChannel>(this INotificationDispatcherBuilder builder)
            where TNotificationChannel : INotificationChannel
        {
            builder.TryRegister(typeof(TNotificationChannel));
            return builder;
        }

        public static INotificationDispatcherBuilder WithAllAvailableChannels(
            this INotificationDispatcherBuilder builder,
            Action<NotificationChannelsLoadingOptions> optionsBuilder = null)
        {
            if (builder is NotificationDispatcherBuilder concreteBuilder)
            {
                var options = new NotificationChannelsLoadingOptions();
                optionsBuilder?.Invoke(options);
                concreteBuilder.LoadNotificationChannels(options);
            }

            return builder;
        }
    }
}
