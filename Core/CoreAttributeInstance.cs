using System;
using System.Collections.Generic;
using System.Reflection;

namespace Linql.ModelGenerator.Core
{
    public class CoreAttributeInstance
    {
        public string TypeName { get; set; }

        public string Module { get; set; }

        public string NameSpace { get; set; }

        public Dictionary<string, object> Arguments { get; set; }

        public override string ToString()
        {
            return $"{this.Module}.{this.TypeName}";
        }

    }
}
