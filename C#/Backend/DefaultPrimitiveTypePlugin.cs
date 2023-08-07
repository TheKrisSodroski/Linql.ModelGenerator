using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.CSharp.Backend
{
    public class DefaultPrimitiveTypePlugin : IPrimitiveTypePlugin
    {
        private static List<Type> AdditionalPrimitives = new List<Type>()
            {
                typeof(Decimal),
                typeof(string),
                typeof(object),
                typeof(DateTime),
            };


        public bool IsPrimitiveType(Type Type)
        {
            return DefaultPrimitiveTypePlugin.AdditionalPrimitives.Contains(Type);
        }
    }
}
