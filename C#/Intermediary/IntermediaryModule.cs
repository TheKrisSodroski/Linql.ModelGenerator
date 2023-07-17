using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryModule
    {
        public string BaseLanguage { get; set; }

        public string ModuleName { get; set; }

        public List<IntermediaryClass> Classes { get; set; } = new List<IntermediaryClass>();

    }
}
