using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WatchCat.Adapters.Core.Extensions;
using WatchCat.Connectors.SignalR;

namespace WatchCat.Adapters.SignalR
{
    public class ConfigureSignalRAdapter : IConfigureAdapter
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var options = app.ApplicationServices.GetRequiredService<SignalRConnectorOptions>();

            app.UseSignalR(configure =>
            {
                if (options.ConfigureHub == null) configure.MapHub<DispatcherHub>(options.Path);
                else configure.MapHub<DispatcherHub>(options.Path, options.ConfigureHub);
            });
        }
    }
}
