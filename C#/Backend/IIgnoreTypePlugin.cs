using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.Backend
{
    public interface IIgnoreTypePlugin
    {
        bool IgnoreType(Type Type);

        bool IgnoreInterface(Type Type);

    }
}
