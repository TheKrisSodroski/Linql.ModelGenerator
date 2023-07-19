using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.Backend
{
    public class DefaultIgnoreTypePlugin : IIgnoreTypePlugin
    {
        public bool IgnoreInterface(Type Type)
        {
            return Type.Assembly == typeof(IComparable).Assembly;
        }

        public bool IgnoreType(Type Type)
        {
            return false;
        }
    }
}
