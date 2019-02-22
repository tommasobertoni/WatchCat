using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WatchCat.Adapters.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAdapters(
            this IServiceCollection services,
            IEnumerable<Assembly> assemblies)
        {
            var config = new AdaptersConfig(services);

            var context = new AdaptersContextService(config, assemblies);
            services.AddSingleton(context);
            services.AddMediatR(assemblies?.ToArray());

            ConfigureAdapterServices(context);
        }

        private static void ConfigureAdapterServices(AdaptersContextService context)
        {
            var configureAdapterServicesType = typeof(IConfigureAdapterServices);

            var configurators = context.TargetAssemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t =>
                    t != configureAdapterServicesType &&
                    configureAdapterServicesType.IsAssignableFrom(t) &&
                    !t.IsAbstract)
                .ToList();

            foreach (var t in configurators)
            {
                var configurator = Activator.CreateInstance(t) as IConfigureAdapterServices;
                configurator.ConfigureServices(context.Config.Services);
            }
        }
    }
}
