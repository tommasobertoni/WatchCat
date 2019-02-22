using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WatchCat.Adapters.Core.Extensions
{
    public class AdaptersConfig
    {
        public IServiceCollection Services { get; }

        public IApplicationBuilder AppBuilder { get; set; }

        internal AdaptersConfig(IServiceCollection services)
        {
            this.Services = services;
        }
    }
}
