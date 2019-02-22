using MediatR;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatchCat.Adapters.Core.Extensions;

namespace WatchCat.Adapters.SignalR
{
    public static class AdaptersConfigExtensions
    {
        public static void SignalRConnector(this AdaptersConfig config, Action<SignalRConnectorOptions> configure)
        {
            var options = config.AppBuilder.ApplicationServices.GetService<SignalRConnectorOptions>();
            configure?.Invoke(options);
        }
    }
}
