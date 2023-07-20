using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryAttribute : IntermediaryType
    {
        public List<IntermediaryArgument> Arguments { get; set; }

        public override string ToString()
        {
            return $"[{base.ToString()}]";
        }


    }
}
