using Linql.ModelGenerator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.ModelGenerator.Typescript.Frontend
{
    public class PropertyMapPlugin: TypescriptGeneratorPlugin
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
                if (Type.IsClass && Type.Properties?.Count > 0)
                {
                    string mapType = $"string, string | (new () => any)";
                    pluginText.Add($"\tpublic static{overrideModifier}PropertyMap = new Map<{mapType}>([");
                    List<string> properties = new List<string>();
                    Type.Properties.ForEach(r =>
                    {
                        bool isPrimitive = r.Type.IsPrimitive;
                        bool isListType = Generator.IsListType(r.Type);
                        string type = "";

                        if (isListType)
                        {
                            type = Generator.BuildGenericType(r.Type.GenericArguments.First());
                        }
                        else
                        {
                            type = Generator.BuildGenericType(r.Type);
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
