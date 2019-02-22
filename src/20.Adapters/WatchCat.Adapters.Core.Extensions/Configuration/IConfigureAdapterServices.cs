using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WatchCat.Adapters.Core.Extensions
{
    public interface IConfigureAdapterServices
    {
        void ConfigureServices(IServiceCollection services);
    }
}
