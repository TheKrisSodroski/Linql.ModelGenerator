using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryType
    {
        public string TypeName { get; set; }

        public List<IntermediaryType> GenericArguments { get; set; }
        public IntermediaryModule Module { get; set; }
        public IntermediaryType TypeConstraint { get; set; }

        public bool IsClass { get; set; }

        public bool IsInterface { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsGenericType { get; set; }

        public bool IsPrimitive { get; set; }

        public List<IntermediaryProperty> Properties { get; set; } = new List<IntermediaryProperty>();

        public List<IntermediaryAttribute> Attributes { get; set; } = new List<IntermediaryAttribute>();

    }
}
