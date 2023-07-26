using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Module1.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AttributePropertyOnly : Attribute
    {
        public string String { get; set; }
        public int Int { get; set; }

        public AttributePropertyOnly(string String = "String", int Int = 5) 
        {
            this.String = String;
            this.Int = Int;
        }
    }
}
