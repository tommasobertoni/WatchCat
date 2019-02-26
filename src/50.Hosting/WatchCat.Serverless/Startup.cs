using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using WatchCat.Core.Events;
using WatchCat.Serverless.Orchestration;
using WatchCat.Connectors.Slack;
using WatchCat.Serverless.Components;
using System.Threading.Tasks;

namespace WatchCat.Serverless
{
    internal class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            services.AddSingleton<Configuration>();

            services.AddSingleton<OrchestratorFactory>(sp =>
            {
                var log = sp.GetService<ILogger<OrchestratorFactory>>();
                var factory = OrchestratorFactory.CreateAsync(log: log).GetAwaiter().GetResult();
                return factory;
            });

            services.AddSingleton<EventsExtractor>();
            services.AddSingleton<Storage>();
            services.AddSingleton<SlackConnector>();

#if TEST
            services.AddSingleton<ISlackWebHookClient>(sp =>
            {
                var log = sp.GetService<ILogger<Connectors.Stubs.Slack.SlackWebHookClientStub>>();
                return new Connectors.Stubs.Slack.SlackWebHookClientStub(log);
            });
#else
            services.AddSingleton<ISlackWebHookClient>(sp =>
            {
                var connector = sp.GetRequiredService<SlackConnector>();
                var configuration = sp.GetRequiredService<Configuration>();
                return connector.CreateWebHookClient(configuration.SlackChannelWebHook);
            });
#endif
        }

        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // Create cloud storage instances
            var storage = serviceProvider.GetRequiredService<Storage>();
            var queue = await storage.GetBufferQueueAsync(createIfNotExists: true);
        }
    }
}
