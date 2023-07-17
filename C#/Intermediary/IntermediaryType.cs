using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryType
    {
        public string TypeName { get; set; }

        public List<IntermediaryType> GenericArguments { get; set; }

        public string ModuleLocation { get; set; }
    }
}
