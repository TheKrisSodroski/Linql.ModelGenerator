using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryEnum : IntermediaryType
    {
        public Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>();
        
        public override string ToString()
        {
            return $"Enum: {base.ToString()}";
        }


    }
}
