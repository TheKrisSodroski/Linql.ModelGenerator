using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.CSharp.Backend
{
    public interface IIgnoreTypePlugin
    {
        bool IgnoreType(Type Type);

        bool IgnoreInterface(Type Type);

    }
}
