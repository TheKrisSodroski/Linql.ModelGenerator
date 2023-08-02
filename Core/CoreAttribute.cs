using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Core
{
    public class CoreAttribute : CoreType
    {
        public List<CoreArgument> RequiredArguments { get; set; }

        public List<CoreArgument> OptionalArguments { get; set; }

        public List<string> Targets { get; set; }

        public override string ToString()
        {
            return $"[{base.ToString()}]";
        }


    }
}
