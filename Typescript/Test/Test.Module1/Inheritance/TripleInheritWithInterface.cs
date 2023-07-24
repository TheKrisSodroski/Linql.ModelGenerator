using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Module1.Inheritance
{
    public abstract class TripleInheritWithInterface : DoubleInheritAbstract, IPrimitiveInterface
    {
       public string CustomField2 { get; set; }
    }
}
