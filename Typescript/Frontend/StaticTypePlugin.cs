using Linql.ModelGenerator.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.Typescript.Frontend
{
    public class StaticTypePlugin: TypescriptGeneratorPlugin
    {
        public override List<string> BeforePropertiesBuilt(CoreType Type, LinqlModelGeneratorTypescriptFrontend Generator)
        {
            List<string> pluginText = new List<string>();

            if(!(Type is CoreAttribute))
            {
                string overrideModifier = " ";

                if(Type.BaseClass != null)
                {
                    overrideModifier = " override ";
                };
                if (Type.IsClass)
                {
                    pluginText.Add($"\tpublic static{overrideModifier}Type = \"{Generator.GetTypeName(Type)}\";");
                }
            }
           
            return pluginText;
        }
    }
}
