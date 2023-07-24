using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Module1.Attributes
{
    public class AttributeWithConstructor : Attribute
    {
        public string String { get; set; }
        public int Int { get; set; }

        public AttributeWithConstructor(string String, int Int) 
        {
            this.String = String;
            this.Int = Int;
        }
    }
}
