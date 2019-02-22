using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatchCat.Connectors.SignalR;

namespace WatchCat.Adapters.SignalR
{
    public class SignalRMethodRoutes
    {
        private List<SignalRMethodRouter> _routes = new List<SignalRMethodRouter>();

        public void Add(string method, params Type[] notificationTypes)
        {
            if (_routes.Any(r => r.Method?.ToLower() == method?.ToLower()))
                throw new Exception($"A route with for the method {method} already exists.");

            if (notificationTypes?.Any() == true)
            {
                var notificationType = typeof(INotification);
                if (notificationTypes.Any(t => t == notificationType))
                    throw new Exception($"Cannot add {notificationType.Name}.");

                var invalidTypes = notificationTypes.Where(t => !notificationType.IsAssignableFrom(t));
                if (invalidTypes.Any())
                    throw new Exception($"Cannot add {invalidTypes.First().Name} because is not of type {notificationType.Name}.");
            }

            _routes.Add(new SignalRMethodRouter(method, notificationTypes));
        }

        public List<SignalRMethodRouter> AsList() => _routes;
    }
}
