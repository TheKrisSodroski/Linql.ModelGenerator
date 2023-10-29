using Linql.ModelGenerator.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.Typescript.Frontend
{
    public abstract class TypescriptGeneratorPlugin
    {
        public virtual List<string> BeforePropertiesBuilt(CoreType Type, LinqlModelGeneratorTypescriptFrontend Generator)
        {
            return new List<string>();
        }
    }
}
