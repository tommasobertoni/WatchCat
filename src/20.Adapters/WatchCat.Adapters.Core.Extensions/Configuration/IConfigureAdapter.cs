using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace WatchCat.Adapters.Core.Extensions
{
    public interface IConfigureAdapter
    {
        void Configure(IApplicationBuilder app, IHostingEnvironment env);
    }
}
