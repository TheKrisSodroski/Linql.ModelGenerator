using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Module1.Attributes
{
    [AttributeWithConstructor("Class", 2)]
    [AttributeWithDefaults()]
    [BasicAttribute]
    public class AttributeInstanceClass
    {
        [AttributeWithConstructor("Property", 2)]
        [AttributeWithDefaults()]
        [BasicAttribute]
        public string String { get; set; }
    }
}
