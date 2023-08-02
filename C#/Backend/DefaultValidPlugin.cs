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

        private static List<Type> AnyTypes = new List<Type>()
            {
                typeof(TimeSpan),
                typeof(Type)
            };



        public bool IsValidType(Type Type)
        {
            bool linqlBaseIgnore = !DefaultValidPlugin.AssembliesToIgnore.Contains(Type.Assembly) && Type.GetCustomAttribute<LinqlGenIngore>() == null;
            return linqlBaseIgnore && !Type.Name.Contains(">c");
        }

        public bool IsValidProperty(Type Type, PropertyInfo PropertyInfo)
        {
            return PropertyInfo.GetCustomAttribute<LinqlGenIngore>() == null;
        }

        public bool IsObjectType(Type Type)
        {
            return DefaultValidPlugin.AnyTypes.Contains(Type);
        }

    }
}
