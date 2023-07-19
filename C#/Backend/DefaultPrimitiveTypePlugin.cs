using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.Backend
{
    public class DefaultPrimitiveTypePlugin : IPrimitiveTypePlugin
    {
        public bool IsPrimitiveType(Type Type)
        {
            if(Type == typeof(string))
            {
                return true;
            }

            return false;
        }
    }
}
