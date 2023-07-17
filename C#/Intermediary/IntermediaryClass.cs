using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryClass
    {
        public string ClassName { get; set; }

        public List<IntermediaryType> GenericArguments { get; set; }
    }
}
