using Linql.ModelGenerator.CSharp.Backend;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Linql.EntityFramework
{
    public class LinqlEFCoreModuleOverride : IModuleOverridePlugin
    {

        protected List<Type> WhiteListedTypes { get; set; } = new List<Type>()
        {
            typeof(PrimaryKeyAttribute)
        };

        public bool IsValidProperty(Type Type, PropertyInfo PropertyInfo)
        {
            return true;
        }

        public bool IsValidType(Type Type)
        {
            if(!this.WhiteListedTypes.Contains(Type) && Type.Assembly == typeof(IndexAttribute).Assembly)
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
