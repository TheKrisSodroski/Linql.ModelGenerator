using Linql.ModelGenerator.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.Typescript.Frontend
{
    public class StaticTypePlugin: TypescriptGeneratorPlugin
    {
        protected string PropertyName { get; set; }

        public StaticTypePlugin(string PropertyName = "Type")
        {
            this.PropertyName = PropertyName;
        }


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
                    pluginText.Add($"\tpublic static{overrideModifier}{this.PropertyName} = \"{Generator.GetTypeName(Type)}\";");
                }
            }
           
            return pluginText;
        }
    }
}
