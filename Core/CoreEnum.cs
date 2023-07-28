using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Core
{
    public class CoreEnum : CoreType
    {
        public Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>();
        
        public override string ToString()
        {
            return $"Enum: {base.ToString()}";
        }


    }
}
