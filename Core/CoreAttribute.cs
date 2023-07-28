using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Core
{
    public class CoreAttribute : CoreType
    {
        public List<CoreArgument> Arguments { get; set; }

        public List<string> Targets { get; set; }

        public override string ToString()
        {
            return $"[{base.ToString()}]";
        }


    }
}
