using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Linql.ModelGenerator.CSharp.Backend
{
    public interface IModuleOverridePlugin
    {
        bool IsValidType(Type Type);

        bool IsValidProperty(Type Type, PropertyInfo PropertyInfo);

        bool IsObjectType(Type Type);

        string ModuleVersionOverride(Assembly Assembly);

    }
}
