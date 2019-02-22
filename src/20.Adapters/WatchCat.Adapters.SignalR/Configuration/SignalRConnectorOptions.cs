using Microsoft.AspNetCore.Http.Connections;
using System;
using System.Collections.Generic;
using System.Text;

namespace WatchCat.Adapters.SignalR
{
    public class SignalRConnectorOptions
    {
        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(Path));

                if (!value.StartsWith("/"))
                    value = $"/{value}";

                _path = value;
            }
        }

        public Action<HttpConnectionDispatcherOptions> ConfigureHub { get; set; }

        public SignalRMethodRoutes Routes { get; } = new SignalRMethodRoutes();

        public SignalRConnectorOptions()
        {
            this.Path = "/hub/connector";
        }
    }
}
