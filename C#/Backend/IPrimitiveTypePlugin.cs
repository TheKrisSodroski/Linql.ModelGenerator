using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.Backend
{
    public interface IPrimitiveTypePlugin
    {
        bool IsPrimitiveType(Type Type);

    }
}
