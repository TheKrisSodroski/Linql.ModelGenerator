using Linql.ModelGenerator.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.Typescript.Frontend
{
    /// <summary>
    /// Will create a default instance property called $type set to the static Type member of the constructor. 
    /// This is used for the not yet standardize moethod of Polymorphic Serialization/Deserialization of json. 
    /// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism#polymorphic-type-discriminators
    /// </summary>
    public class PolymorphicTypePlugin : TypescriptGeneratorPlugin
    {
        protected string PropertyName { get; set; }

        public PolymorphicTypePlugin(string PropertyName = "$type")
        {
            this.PropertyName = PropertyName;
        }


        public override List<string> BeforePropertiesBuilt(CoreType Type, LinqlModelGeneratorTypescriptFrontend Generator)
        {
            List<string> pluginText = new List<string>();

            if(!(Type is CoreAttribute))
            {
                string overrideModifier = "";

                if(Type.BaseClass != null)
                {
                    overrideModifier = "override ";
                };
                if (Type.IsClass)
                {
                    pluginText.Add($"\tpublic {overrideModifier}{this.PropertyName} = {Generator.GetTypeName(Type)}.Type;");
                }
            }
           
            return pluginText;
        }
    }
}
