using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace WatchCat.Adapters.Core.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAdapters(
            this IApplicationBuilder app,
            IHostingEnvironment env,
            Action<AdaptersConfig> configure = null)
        {
            var context = app.ApplicationServices.GetService<AdaptersContextService>();
            if (context == null)
                throw new InvalidOperationException("Unable to find the required services. Please add all the required services by calling 'IServiceCollection.AddAdapters' inside the call to 'ConfigureServices(...)' in the application startup code.");

            context.Config.AppBuilder = app;

            configure?.Invoke(context.Config);

            ConfigureAdapters(context, app, env);

            return app;
        }

        private static void ConfigureAdapters(AdaptersContextService context, IApplicationBuilder app, IHostingEnvironment env)
        {
            var configureAdaptersType = typeof(IConfigureAdapter);

            var configurators = context.TargetAssemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t =>
                    t != configureAdaptersType &&
                    configureAdaptersType.IsAssignableFrom(t) &&
                    !t.IsAbstract)
                .ToList();

            foreach (var t in configurators)
            {
                var configurator = Activator.CreateInstance(t) as IConfigureAdapter;
                configurator.Configure(app, env);
            };
        }
    }
}
