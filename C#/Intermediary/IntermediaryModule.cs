using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryModule
    {
        public string BaseLanguage { get; set; }

        public string ModuleName { get; set; }

        public List<IntermediaryType> Types { get; set; } = new List<IntermediaryType>();

    }
}
