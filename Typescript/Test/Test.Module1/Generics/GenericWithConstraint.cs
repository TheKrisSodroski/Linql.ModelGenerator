using System;
using System.Collections.Generic;
using System.Text;
using Test.Module1.Inheritance;

namespace Test.Module1.Generics
{
    public class GenericWithConstraint<T, S> where T: IPrimitiveInterface where S: MultipleInterfacesNested, IInterfaceFour, new()
    {
    }
}
