using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WatchCat.Core.Extensions
{
    internal class FileSystemUtils
    {
        public static string GetAppDirectoryPath()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var directoryPath = Path.GetDirectoryName(entryAssembly.Location);
            return directoryPath;
        }
    }
}
