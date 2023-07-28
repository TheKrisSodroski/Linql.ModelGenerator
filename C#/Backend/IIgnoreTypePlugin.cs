using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Linql.ModelGenerator.CSharp.Backend
{
    public interface IIgnoreTypePlugin
    {
        bool IsValidType(Type Type);

        bool IsValidProperty(Type Type, PropertyInfo PropertyInfo);

    }
}
