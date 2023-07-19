using System;
using System.Collections.Generic;
using System.Reflection;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryProperty
    {
        public string PropertyName { get; set; }

        public List<IntermediaryAttribute> Attributes { get; set; } = new List<IntermediaryAttribute>();

        public IntermediaryType Type { get; set; }

        public override string ToString()
        {
            return $"{this.PropertyName} - {this.Type.ToString()}";
        }

    }
}
