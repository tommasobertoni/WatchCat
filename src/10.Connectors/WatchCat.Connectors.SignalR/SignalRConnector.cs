using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchCat.Connectors.SignalR
{
    public class DispatcherHub : Hub
    {
    }

    public class SignalRConnector
    {
        private readonly IHubContext<DispatcherHub> _hub;
        private List<SignalRMethodRouter> _routes;

        public SignalRConnector(IHubContext<DispatcherHub> hub, List<SignalRMethodRouter> routes = null)
        {
            _hub = hub;
            this.UseRoutes(routes);
        }

        public void UseRoutes(List<SignalRMethodRouter> routes)
        {
            _routes = routes;
        }

        public async Task SendAsync(object content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            List<string> methods = GetMethodsToInvoke(content);

            if (methods.Any())
            {
                await Task.WhenAll(methods.Select(m => _hub.Clients.All.SendAsync(m, content))).ConfigureAwait(false);
            }
        }

        private List<string> GetMethodsToInvoke(object content)
        {
            var methodsToInvoke = new List<string>();

            if (_routes?.Any() != true) return methodsToInvoke;

            var contentType = content.GetType();

            foreach (var route in _routes)
            {
                if (route.AcceptedMessageTypes.Contains(contentType) ||
                    !route.AcceptedMessageTypes.Any())
                {
                    methodsToInvoke.Add(route.Method);
                }
            }

            return methodsToInvoke;
        }
    }
}
