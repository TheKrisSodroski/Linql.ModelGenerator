using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.CSharp.Backend
{
    public interface IPrimitiveTypePlugin
    {
        bool IsPrimitiveType(Type Type);

    }
}
