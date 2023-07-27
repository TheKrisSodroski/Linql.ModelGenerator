using System;
using System.Collections.Generic;
using System.Text;
using Test.Module1.Generics;
using Test.Module1.Inheritance;

namespace Test.Module1.Complex
{
    public class ComplexClass: GenericTwo<GenericOne<string>, GenericOne<int>>
    {
        public IPrimitiveInterface PrimitiveInterface { get; set; }

        public GenericOne<PrimitiveAbstract> GenericOneWithClass { get; set; }

        public List<GenericTwo<PrimitiveAbstract, IInterfaceFour>> DoubleGeneric { get; set; }
    }
}
