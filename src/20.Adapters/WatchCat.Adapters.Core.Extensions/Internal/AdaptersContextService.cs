using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WatchCat.Adapters.Core.Extensions
{
    internal class AdaptersContextService
    {
        public AdaptersConfig Config { get; }

        public List<Assembly> TargetAssemblies { get; }

        public AdaptersContextService(AdaptersConfig config, IEnumerable<Assembly> targetAssemblies)
        {
            this.Config = config;
            this.TargetAssemblies = targetAssemblies?.ToList();
        }
    }
}
