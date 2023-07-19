using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Module1.Attributes
{
    public class AttributeWithDefaults : Attribute
    {
        public string String { get; set; }
        public int Int { get; set; }

        public AttributeWithDefaults(string String = "String", int Int = 5) 
        {
            this.String = String;
            this.Int = Int;
        }
    }
}
