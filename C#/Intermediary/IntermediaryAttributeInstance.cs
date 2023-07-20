using System;
using System.Collections.Generic;
using System.Reflection;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryAttributeInstance
    {
        public string TypeName { get; set; }

        public string Module { get; set; }

        public Dictionary<string, object> Arguments { get; set; }

        public override string ToString()
        {
            return $"{this.Module}.{this.TypeName}";
        }

    }
}
