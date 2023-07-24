using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Module1.Inheritance
{
    public interface IInterfaceOne
    {

    }

    public interface IInterfaceTwo 
    { 
    }

    public interface IInterfaceThree
    {

    }

    public interface IInterfaceFour : IInterfaceThree { }

    public class MultipleInterfaces : IInterfaceOne, IInterfaceTwo, IInterfaceThree
    {
       
    }

    public class MultipleInterfacesNested : IInterfaceFour, IInterfaceTwo
    {

    }
}
