using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Linql.ModelGenerator.Backend
{
    public class DefaultIgnoreTypePlugin : IIgnoreTypePlugin
    {
        private static List<Assembly> AssembliesToIgnore = new List<Assembly>()
            {
                typeof(IComparable).Assembly,
                typeof(Attribute).Assembly
            };


        public bool IgnoreInterface(Type Type)
        {
            return DefaultIgnoreTypePlugin.AssembliesToIgnore.Contains(Type.Assembly);
        }

        public bool IgnoreType(Type Type)
        {
            return DefaultIgnoreTypePlugin.AssembliesToIgnore.Contains(Type.Assembly);
        }
    }
}
