using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchCat.Core.Extensions.DependencyInjection
{
    public class NotificationChannelsLoadingOptions
    {
        public bool InAppDirectory { get; set; } = true;

        public List<string> InDirectories { get; set; } = new List<string>();

        public AssemblyInclusionDelegate IncludeAssembly { get; set; }

        public TypeInclusionDelegate IncludeType { get; set; }
    }
}
