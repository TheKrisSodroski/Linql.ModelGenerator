using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Module1.PropertyOverride
{
    public class PropertyOverride : BasePropertyOverride
    {
        public override int Integer { get; set; }

        public int NotOverride { get; set; }
    }
}
