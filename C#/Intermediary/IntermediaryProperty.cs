using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryProperty
    {
        public string PropertyName { get; set; }

        public List<IntermediaryAttribute> Attributes { get; set; } = new List<IntermediaryAttribute>();

        public IntermediaryType Type { get; set; }
    }
}
