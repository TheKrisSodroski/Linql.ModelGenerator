using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Module1.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AttributeClassOnly : Attribute
    {
        public string String { get; set; }
        public int Int { get; set; }

        public AttributeClassOnly(string String = "String", int Int = 5) 
        {
            this.String = String;
            this.Int = Int;
        }
    }
}
