using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WatchCat.Adapters;
using WatchCat.Adapters.Core.Extensions;
using WatchCat.Adapters.SignalR;
using WatchCat.Adapters.Log;

namespace WatchCat.WebApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAdapters(AssembliesToScan());

            services
                .AddMvc(o => o.InputFormatters.Insert(0, new TextInputFormatter()))
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);

            // Local functions

            IEnumerable<Assembly> AssembliesToScan()
            {
                var appDirectoryPath = AssemblyUtils.GetAppDirectoryPath(); 
                var assemblies = AssemblyUtils.GetAssembliesInDirectory(appDirectoryPath);
                return assemblies;
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseAdapters(env, config =>
            {
                config.SignalRConnector(sgnlr =>
                {
                    sgnlr.Path = "/hub/connector";
                    sgnlr.Routes.Add("all");
                    sgnlr.Routes.Add("data", typeof(PayloadNotification));
                    sgnlr.Routes.Add("logs", typeof(LogNotification));
                    sgnlr.Routes.Add("messages", typeof(MessageNotification)); 
                });
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
