using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Core
{
    public class CoreModule
    {
        public string BaseLanguage { get; set; }

        public string ModuleName { get; set; }

        public string Version { get; set; }

        public List<CoreType> Types { get; set; } 

        public override string ToString()
        {
            return $"{this.ModuleName} - {this.BaseLanguage}";
        }


    }
}
