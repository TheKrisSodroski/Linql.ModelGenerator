using Linql.ModelGenerator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.ModelGenerator.Typescript.Frontend
{
    public class PropertyMapPlugin: TypescriptGeneratorPlugin
    {
        protected string GetPropertyMapType(CoreType ParentType, CoreType PropertyType, LinqlModelGeneratorTypescriptFrontend Generator)
        {
            bool isPrimitive = PropertyType.IsPrimitive;
            bool isListType = Generator.IsListType(PropertyType);
            bool? isGeneric = ParentType.GenericArguments?.Any(s => s.TypeName == PropertyType.TypeName);
            string type = "";

            if (isListType)
            {
                type = this.GetPropertyMapType(ParentType, PropertyType.GenericArguments.First(), Generator);
            }
            else if (isGeneric == true)
            {
                type = "any";
            }
            else
            {
                type = Generator.BuildGenericType(PropertyType);
            }

            if (type == "any")
            {
                type = "Object";
            }
            else if (isPrimitive && type != "Date")
            {
                if (type != "Date")
                {
                    type = $"\"{type}\"";
                }
            }

            return type;
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
                if (Type.IsClass && Type.Properties?.Count > 0)
                {
                    string mapType = $"string, string | (new () => any)";
                    pluginText.Add($"\tpublic static{overrideModifier}PropertyMap: Map<{mapType}> = new Map<{mapType}>([");
                    List<string> properties = new List<string>();
                    Type.Properties.ForEach(r =>
                    {
                        string type = this.GetPropertyMapType(Type, r.Type, Generator);
                        properties.Add($"\t\t[\"{r.PropertyName}\", {type}]");
                    });
                    pluginText.Add(String.Join(",\n", properties));
                    pluginText.Add("\t]);");
                }
            }
           
            return pluginText;
        }
    }
}
