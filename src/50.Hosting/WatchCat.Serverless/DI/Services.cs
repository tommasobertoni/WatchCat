using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WatchCat.Serverless.DI
{
    /* Services locator */

    internal class ServiceProvider
    {
        private static readonly IServiceProvider _serviceProvider;

        static ServiceProvider()
        {
            var services = new ServiceCollection();
            var startup = new Startup();
            startup.ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider(true);
            startup.InitializeAsync(_serviceProvider).GetAwaiter().GetResult();
        }

        public static T Resolve<T>() => _serviceProvider.GetService<T>();
    }
}
