using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Linql.ModelGenerator.CSharp.Backend
{
    public class DefaultOverridePlugin : IModuleOverridePlugin
    {
        protected static List<Assembly> AssembliesToIgnore = new List<Assembly>()
            {
                typeof(IComparable).Assembly,
                typeof(Attribute).Assembly,
                typeof(DefaultOverridePlugin).Assembly,
                typeof(JsonIgnoreAttribute).Assembly,
                typeof(StreamContent).Assembly
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
            bool linqlBaseIgnore = !DefaultOverridePlugin.AssembliesToIgnore.Contains(Type.Assembly) && Type.GetCustomAttribute<LinqlGenIngore>() == null;
            return linqlBaseIgnore && !IgnoreIfNameContains.Any(s => Type.Name.Contains(s));
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
