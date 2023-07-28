using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Linql.ModelGenerator.CSharp.Backend
{
    public class DefaultValidPlugin : IIgnoreTypePlugin
    {
        private static List<Assembly> AssembliesToIgnore = new List<Assembly>()
            {
                typeof(IComparable).Assembly,
                typeof(Attribute).Assembly,
                typeof(DefaultValidPlugin).Assembly
            };


        public bool IsValidType(Type Type)
        {
            return !DefaultValidPlugin.AssembliesToIgnore.Contains(Type.Assembly) && Type.GetCustomAttribute<LinqlGenIngore>() == null;
        }

        public bool IsValidProperty(Type Type, PropertyInfo PropertyInfo)
        {
            return PropertyInfo.GetCustomAttribute<LinqlGenIngore>() == null;
        }
    }
}
