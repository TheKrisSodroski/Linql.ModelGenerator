using Linql.ModelGenerator.CSharp.Backend;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Linql.EntityFramework
{
    public class LinqlEntityFrameworkModuleOverride : IModuleOverridePlugin
    {
        public bool IsValidProperty(Type Type, PropertyInfo PropertyInfo)
        {
            return true;
        }

        public bool IsValidType(Type Type)
        {
            if(Type.Assembly == typeof(IndexAttribute).Assembly)
            {
                return false;
            }
            return true;
        }

        public bool IsObjectType(Type Type)
        {
            return false;
        }

        public string ModuleVersionOverride(Assembly Assembly)
        {
            return null;
        }
    }

}
