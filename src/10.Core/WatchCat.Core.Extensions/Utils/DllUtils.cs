using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WatchCat.Core.Extensions
{
    public delegate bool AssemblyInclusionDelegate(string assemblyName);

    public delegate bool TypeInclusionDelegate(Type type);

    internal class DllUtils
    {
        public static IEnumerable<Type> EnumerateTypes(
            string dllsDirectory,
            AssemblyInclusionDelegate dllInclusion = null,
            TypeInclusionDelegate typeInclusion = null)
        {
            var dllPaths = Directory.EnumerateFiles(dllsDirectory, "*.dll");

            var dllsInfo = dllPaths.Select(dllPath => (path: dllPath, name: Path.GetFileName(dllPath)));

            if (dllInclusion != null)
                dllsInfo = dllsInfo.Where(dllInfo => dllInclusion(dllInfo.name));

            var assemblies = dllsInfo.Select(dllInfo => Assembly.LoadFile(dllInfo.path));

            var types = assemblies.SelectMany(assembly => assembly.GetTypes());

            if (typeInclusion != null)
                types = types.Where(type => typeInclusion(type));

            return types;
        }
    }
}
