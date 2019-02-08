using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchCat.Core.Notifications;

namespace WatchCat.Core.Extensions.DependencyInjection
{
    public interface INotificationChannelsBuilder
    {
        bool TryRegister(Type notificationChannelType);
    }

    internal class NotificationChannelsBuilder : INotificationChannelsBuilder
    {
        protected readonly Type _NotificationChannelType = typeof(INotificationChannel<>);
        protected readonly Type _NonGenericNotificationChannelType = typeof(INotificationChannel);

        private readonly HashSet<Type> _registeredNotificationChannelTypes = new HashSet<Type>();

        internal IServiceCollection Services { get; }

        public IEnumerable<Type> RegisteredNotificationChannelTypes => _registeredNotificationChannelTypes;

        public NotificationChannelsBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public bool TryRegister(Type notificationChannelType)
        {
            if (!IsNotificationChannelType(notificationChannelType))
                return false;

            if (IsNotificationChannelTypeRegistered(notificationChannelType))
                return false;

            if (!_registeredNotificationChannelTypes.Contains(notificationChannelType))
                _registeredNotificationChannelTypes.Add(notificationChannelType);

            Services.AddSingleton(notificationChannelType);

            return true;
        }

        internal INotificationChannelsBuilder LoadNotificationChannels()
        {
            var notificationChannelTypes = FindNotificationChannelTypes();
            notificationChannelTypes.ForEach(t => TryRegister(t));

            return this;

            // Local functions

            List<Type> FindNotificationChannelTypes()
            {
                var appDirectoryPath = FileSystemUtils.GetAppDirectoryPath();

                var types = DllUtils.EnumerateTypes(appDirectoryPath,
                    dllInclusion: IncludeAssembly,
                    typeInclusion: IncludeType).ToList();

                return types;
            }

            bool IncludeAssembly(string assemblyName)
            {
                if (assemblyName.StartsWith("System.")) return false;
                if (assemblyName.StartsWith("Microsoft.")) return false;
                return true;
            }

            bool IncludeType(Type type)
            {
                if (type.FullName.StartsWith("System.")) return false;
                if (type.FullName.StartsWith("Microsoft.")) return false;

                return !type.IsAbstract && !type.IsInterface && type.IsPublic &&
                       _NonGenericNotificationChannelType.IsAssignableFrom(type) &&
                       type != _NonGenericNotificationChannelType && type != _NotificationChannelType;
            }
        }

        internal bool IsNotificationChannelType(Type type)
        {
            if (type == null) return false;

            return
                _NotificationChannelType.IsAssignableFrom(type) ||
                _NonGenericNotificationChannelType.IsAssignableFrom(type);
        }

        internal bool IsNotificationChannelTypeRegistered(Type notificationChannelType)
        {
            if (!IsNotificationChannelType(notificationChannelType))
                throw new InvalidOperationException($"{notificationChannelType?.Name} doesn't inherit from {nameof(INotificationChannel)}.");

            if (notificationChannelType == null) return false;
            if (_NotificationChannelType == notificationChannelType) return false;
            if (_NonGenericNotificationChannelType == notificationChannelType) return false;

            return Services.Any(sdesc => sdesc.ImplementationType == notificationChannelType);
        }
    }
}
