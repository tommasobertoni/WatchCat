using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using WatchCat.Adapters.Core.Extensions;
using WatchCat.Connectors.SignalR;

namespace WatchCat.Adapters.SignalR
{
    public class ConfigureSignalRAdapterServices : IConfigureAdapterServices
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<SignalRConnectorOptions>();
            services.AddTransient<SignalRConnector>(sp =>
            {
                var hub = sp.GetRequiredService<IHubContext<DispatcherHub>>();
                var options = sp.GetRequiredService<SignalRConnectorOptions>();
                return new SignalRConnector(hub, options.Routes?.AsList());
            });
        }
    }
}
