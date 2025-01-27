using Linql.ModelGenerator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.ModelGenerator.Typescript.Frontend
{
    public class PropertyMapPlugin : TypescriptGeneratorPlugin
    {
        protected string PropertyName { get; set; }

        public PropertyMapPlugin(string PropertyName = "LinqlPropertyMap")
        {
            this.PropertyName = PropertyName;
        }

        protected string GetPropertyMapType(CoreType ParentType, CoreType PropertyType, LinqlModelGeneratorTypescriptFrontend Generator)
        {
            bool isPrimitive = PropertyType.IsPrimitive;
            bool isListType = Generator.IsListType(PropertyType);

            bool isFileType = isListType && PropertyType.GenericArguments.FirstOrDefault()?.TypeName == typeof(byte).Name;
            bool isEnumType = PropertyType is CoreEnum;
            bool? isGeneric = ParentType.GenericArguments?.Any(s => s.TypeName == PropertyType.TypeName);
            string type = "";

            if(isFileType)
            {
                type = "Uint8Array";
            }
            else if (isListType)
            {
                type = this.GetPropertyMapType(ParentType, PropertyType.GenericArguments.First(), Generator);
            }
            else if (isEnumType)
            {
                type = "any";
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

            if (!(Type is CoreAttribute))
            {
                string overrideModifier = "";

                if (Type.BaseClass != null)
                {
                    overrideModifier = "override";
                };
                if (Type.IsClass && Type.Properties?.Count > 0)
                {
                    string mapType = $"string, string | (new () => any) | (abstract new() => any)";

                    List<string> properties = new List<string>();
                    Type.Properties.ForEach(r =>
                    {
                        string type = this.GetPropertyMapType(Type, r.Type, Generator);
                        properties.Add($"\t\t\t\t[\"{r.PropertyName}\", {type}]");
                    });
                    string propertyBlock = String.Join(",\n", properties);

                    string privatePropertyName = $"{Type.TypeName}{this.PropertyName}";

                    string mergeMap = "";

                    if(Type.BaseClass != null)
                    {
                        mergeMap = $",...{Type.BaseClass.TypeName}.{this.PropertyName}";
                    }

                    string protectedProperty =
                        $@"
    protected static {privatePropertyName}: Map<{mapType}> | undefined;";

                    string propertyMapCode =
    $@"
    {protectedProperty}
    public static {overrideModifier} get {this.PropertyName}()
    {{
        if(!this.{privatePropertyName})
        {{
            this.{privatePropertyName} = new Map<{mapType}>([
{propertyBlock}
            {mergeMap}]
            );
        }}
        return this.{privatePropertyName};
    }}";

                    pluginText.Add(propertyMapCode);
                }
            }

            return pluginText;
        }
    }
}
