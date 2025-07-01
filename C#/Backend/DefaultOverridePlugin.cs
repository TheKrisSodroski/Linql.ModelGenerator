using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linql.ModelGenerator.CSharp.Backend
{
    public class DefaultOverridePlugin : IModuleOverridePlugin
    {
        protected Dictionary<string, string> DepVersion { get; set; }

        protected static List<Assembly> AssembliesToIgnore = new List<Assembly>()
            {
                typeof(IComparable).Assembly,
                typeof(Attribute).Assembly,
                typeof(DefaultOverridePlugin).Assembly,
                typeof(JsonIgnoreAttribute).Assembly,
                typeof(StreamContent).Assembly
            };

        protected static List<string> NamespacesToIgnore = new List<string>()
        {
            "Microsoft.CodeAnalysis",
            "System.Runtime.CompilerServices"

        };

        protected static List<Type> AnyTypes = new List<Type>()
            {
                typeof(TimeSpan),
                typeof(Type)
            };

        protected static List<string> IgnoreIfNameContains = new List<string>()
        {
            ">c",
            "__"
        };

        public bool IsValidType(Type Type)
        {
            bool linqlBaseIgnore = !DefaultOverridePlugin.AssembliesToIgnore.Contains(Type.Assembly)
                && !DefaultOverridePlugin.NamespacesToIgnore.Contains(Type.Namespace)
                && Type.GetCustomAttribute<LinqlGenIngore>() == null;
            return linqlBaseIgnore
                && !IgnoreIfNameContains.Any(s => Type.Name.Contains(s));
        }

        private bool isSealed(Type Type)
        {
            return Type.IsSealed;
        }

        public bool IsValidProperty(Type Type, PropertyInfo PropertyInfo)
        {
            return PropertyInfo.GetCustomAttribute<LinqlGenIngore>() == null;
        }

        public bool IsObjectType(Type Type)
        {
            return DefaultOverridePlugin.AnyTypes.Contains(Type);
        }

        public string ModuleVersionOverride(Assembly Assembly)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string assemblyName = Assembly.GetName().Name;

                if (this.DepVersion == null)
                {
                    string dllLocation = Assembly.Location;
                    string dllFolder = Path.GetDirectoryName(dllLocation);
                    List<string> depFiles = Directory.GetFiles(dllFolder, "*.deps.json").ToList();
                    string depFile = depFiles.FirstOrDefault();
                    string depFileText = File.ReadAllText(depFile);
                    var depFileObj = JsonSerializer.Deserialize<LinuxDepFile>(depFileText);

                    this.DepVersion = depFileObj.libraries.Keys.ToDictionary(r => r.Split('/')[0], r => r.Split('/')[1]);
                }

                string versionNumber = this.DepVersion[assemblyName];

                return versionNumber;
            }
            else
            {
                AssemblyInformationalVersionAttribute version = Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                string informationVersion = version.InformationalVersion;

                if (informationVersion.Split('.').Count() > 3)
                {
                    informationVersion = String.Join(".", informationVersion.Split('.').Take(3));
                }

                return informationVersion.Split('+')[0];
            }

        }
    }

    internal class LinuxDepFile
    {
        public Dictionary<string, Dictionary<string, object>> libraries { get; set; }
    }

}
