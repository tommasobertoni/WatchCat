using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WatchCat.WebApp
{
    internal class AssemblyUtils
    {
        public static string GetAppDirectoryPath()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var directoryPath = Path.GetDirectoryName(entryAssembly.Location);
            return directoryPath;
        }

        public static List<Assembly> GetAssembliesInDirectory(string directoryPath)
        {
            var assemblies = Directory.EnumerateFiles(directoryPath, "*.dll")
                .Select(dllPath => Assembly.LoadFrom(dllPath))
                .ToList();

            return assemblies;
        }

        public static List<Assembly> GetReferencedAssemblies()
        {
            var assemblies = Assembly
                .GetEntryAssembly()
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Union(new[] { Assembly.GetEntryAssembly() })
                .ToList();

            return assemblies;
        }

        public List<Type> GetTypes(IEnumerable<Assembly> assemblies)
        {
            var types = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .ToList();

            return types;
        }
    }
}
