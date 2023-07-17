using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryGenericType : IntermediaryType
    {
        public IntermediaryType TypeConstraint { get; set; }
    }
}
