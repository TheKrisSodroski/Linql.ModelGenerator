using System;
using System.Collections.Generic;

namespace Linql.ModelGenerator.Core
{
    public class CoreArgument
    {
        public string ArgumentName { get; set; }

        public object DefaultValue { get; set; }

        public CoreType Type { get; set; }

        public override string ToString()
        {
            return $"{this.ArgumentName} - {this.Type.ToString()}";
        }

    }
}
