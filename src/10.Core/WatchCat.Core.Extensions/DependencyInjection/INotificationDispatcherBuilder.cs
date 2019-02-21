using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchCat.Core.Notifications;

namespace WatchCat.Core.Extensions.DependencyInjection
{
    public interface INotificationDispatcherBuilder
    {
        bool TryRegister(Type notificationChannelType);
    }

    internal delegate void NotificationChannelTypeRegistered(Type notificationChannelType);

    internal class NotificationDispatcherBuilder : INotificationDispatcherBuilder
    {
        protected readonly Type _GenericNotificationChannelType = typeof(INotificationChannel<>);
        protected readonly Type _NonGenericNotificationChannelType = typeof(INotificationChannel);

        private readonly HashSet<Type> _registeredNotificationChannelTypes = new HashSet<Type>();

        public event NotificationChannelTypeRegistered OnNotificationChannelTypeRegistered;

        public IEnumerable<Type> RegisteredNotificationChannelTypes => _registeredNotificationChannelTypes;

        public bool TryRegister(Type notificationChannelType)
        {
            if (!IsNotificationChannelType(notificationChannelType))
                return false;

            if (IsNotificationChannelTypeRegistered(notificationChannelType))
                return false;

            _registeredNotificationChannelTypes.Add(notificationChannelType);
            OnNotificationChannelTypeRegistered?.Invoke(notificationChannelType);

            return true;
        }

        internal INotificationDispatcherBuilder LoadNotificationChannels(NotificationChannelsLoadingOptions options)
        {
            var notificationChannelTypes = FindNotificationChannelTypes();
            notificationChannelTypes.ForEach(t => TryRegister(t));

            return this;

            // Local functions

            List<Type> FindNotificationChannelTypes()
            {
                var typesSet = new HashSet<Type>();

                var directoriesToScanSet = new HashSet<string>();

                if (options.InAppDirectory)
                    directoriesToScanSet.Add(FileSystemUtils.GetAppDirectoryPath());

                if (options.InDirectories?.Any() == true)
                    options.InDirectories.ForEach(d => directoriesToScanSet.Add(d));

                if (directoriesToScanSet.Any())
                {
                    foreach (var directory in directoriesToScanSet)
                    {
                        var includeAssembly = options.IncludeAssembly == null
                            ? new AssemblyInclusionDelegate(assemblyName => !IsSystemLibrary(assemblyName))
                            : new AssemblyInclusionDelegate(assemblyName => !IsSystemLibrary(assemblyName) && options.IncludeAssembly(assemblyName));

                        var typesInDirectory = DllUtils.EnumerateTypes(directory,
                            dllInclusion: includeAssembly,
                            typeInclusion: options.IncludeType).ToList();

                        typesInDirectory.ForEach(t => typesSet.Add(t));
                    }
                }

                return typesSet.ToList();
            }

            bool IsSystemLibrary(string assemblyName) =>
                assemblyName.StartsWith("System.") || assemblyName.StartsWith("Microsoft.");
        }

        internal bool IsNotificationChannelType(Type type)
        {
            if (type == null) return false;

            bool isNotificationChannelType =
                !(type.IsAbstract || type.IsInterface || !type.IsPublic) &&
                type != _GenericNotificationChannelType && type != _NonGenericNotificationChannelType &&
                type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == _GenericNotificationChannelType);

            return isNotificationChannelType;
        }

        internal bool IsNotificationChannelTypeRegistered(Type notificationChannelType)
        {
            if (notificationChannelType == null) return false;
            if (_GenericNotificationChannelType == notificationChannelType) return false;
            if (_NonGenericNotificationChannelType == notificationChannelType) return false;

            return _registeredNotificationChannelTypes.Contains(notificationChannelType);
        }
    }
}
