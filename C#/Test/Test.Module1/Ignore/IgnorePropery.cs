using Linql.ModelGenerator.CSharp.Backend;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Module1.Ignore
{
    public class IgnorePropery
    {
        [LinqlGenIngore]
        public string IgnoreProperty { get; set; }

        public string NotIgnoreProperty { get; set; }
    }
}
