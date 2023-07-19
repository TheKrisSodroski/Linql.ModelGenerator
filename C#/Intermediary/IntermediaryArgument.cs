using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryArgument
    {
        public string ArgumentName { get; set; }

        public object DefaultValue { get; set; }

        public IntermediaryType Type { get; set; }

        public override string ToString()
        {
            return $"{this.ArgumentName} - {this.Type.ToString()}";
        }

    }
}
