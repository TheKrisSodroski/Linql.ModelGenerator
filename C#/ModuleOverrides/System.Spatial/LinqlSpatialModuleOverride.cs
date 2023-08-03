using Linql.ModelGenerator.CSharp.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Spatial;

namespace Linql.System.Spatial
{
    public class LinqlSpatialModuleOverride : IModuleOverridePlugin
    {
        public bool IsValidProperty(Type Type, PropertyInfo PropertyInfo)
        {
            return true;
        }

        public bool IsValidType(Type Type)
        {
            return true;
        }

        public bool IsObjectType(Type Type)
        {
            List<Type> objectTypes = new List<Type>()
            {
                typeof(Geography),
                typeof(Geometry)
            };
            return objectTypes.Contains(Type);
        }

        public string ModuleVersionOverride(Assembly Assembly)
        {
            if (Assembly == typeof(Geography).Assembly)
            {
                AssemblyInformationalVersionAttribute version = Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                string informationVersion = version.InformationalVersion;

                if (informationVersion.Split('.').Count() > 3)
                {
                    informationVersion = String.Join(".", informationVersion.Split('.').Take(3));
                }

                return informationVersion.Split('+')[0] + "-alpha1";
            }
            else
            {
                return null;
            }
        }
    }

}
