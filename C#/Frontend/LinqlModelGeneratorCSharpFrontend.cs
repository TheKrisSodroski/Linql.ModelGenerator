﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Linql.ModelGenerator.Core;

namespace Linql.ModelGenerator.CSharp.Frontend
{
    public partial class LinqlModelGeneratorCSharpFrontend : LinqlFrontendModelGenerator
    {
        public LinqlModelGeneratorCSharpFrontend(string CoreJson, string ProjectPath = null) : base(CoreJson, ProjectPath) { }
        public LinqlModelGeneratorCSharpFrontend(CoreModule Module, string ProjectPath = null) : base(Module, ProjectPath) { }

        protected override void AddAdditionalModules(Dictionary<string, string> AdditionalModules)
        {
            XElement itemGroup = new XElement("ItemGroup");
            AdditionalModules.Select(r =>
            {
                XElement package = new XElement("PackageReference");
                package.SetAttributeValue("Include", r.Key);
                package.SetAttributeValue("Version", r.Value);
                return package;
            }).ToList()
                .ForEach(r => itemGroup.Add(r));
           

            string projectPath = Path.Combine(this.ProjectPath, this.Module.ModuleName, $"{this.Module.ModuleName}.csproj");

            XDocument doc = XDocument.Load(projectPath);
            doc.Element("Project").Add(itemGroup);
            string xmlDoc = doc.ToString();
            File.WriteAllText(projectPath, xmlDoc);
        }

        protected override void CreateProject()
        {
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo("dotnet");
            processStartInfo.Arguments = $"new classlib -o {this.Module.ModuleName} -f netstandard2.0";
            processStartInfo.WorkingDirectory = this.ProjectPath;
            processStartInfo.CreateNoWindow = false;
            process.StartInfo = processStartInfo;
            process.Start();

            process.WaitForExit();

            File.Delete(Path.Combine(this.ProjectPath, this.Module.ModuleName, "Class1.cs"));
        }

        protected override void CreateType(CoreType Type)
        {
            List<string> fileText = this.Usings.ToList();
            string folder = Type.NameSpace.Replace($"{this.Module.ModuleName}", String.Empty).TrimStart('.');
            string directory = Path.Combine(this.ProjectPath, this.Module.ModuleName, folder);
          
            Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, $"{Type.TypeName}.cs");

            List<string> additionalImports = this.ExtractImports(Type);

            fileText.AddRange(additionalImports.Select(r => $"using {r};"));

            fileText.Add("");
            fileText.Add($"namespace {Type.NameSpace}");
            fileText.Add("{");

            string classType = null;

            if (Type.IsInterface)
            {
                classType = "interface";
            }
            else if (Type.IsAbstract)
            {
                classType = "abstract class";
            }
            else if (Type.IsClass)
            {
                classType = "class";
            }
            else if(Type is CoreEnum Enum)
            {
                classType = "enum";
            }
            else
            {
                throw new Exception($"Unable to determine class type for Type {Type.TypeName}");
            }

            if(Type is CoreAttribute attrTarget)
            {
                List<string> usage = attrTarget.Targets.Select(r => $"{nameof(AttributeTargets)}.{r}").ToList();
                fileText.Add($"\t[AttributeUsage({String.Join("| ", usage)})]");
            }

            if(Type.Attributes != null)
            {
                List<string> attrs = Type.Attributes.Select(r => $"\t{this.BuildAttributeInstance(r)}").ToList();
                fileText.AddRange(attrs);
            }

            string classRegion = $"\tpublic {classType} {this.GetTypeName(Type)}";

            if(Type.IsGenericType)
            {
                classRegion += "<";
                string generics = String.Join(", ", Type.GenericArguments.Select(r => this.GetTypeName(r)));
                classRegion += generics + ">";
            }

            if(Type.BaseClass != null || Type.Interfaces?.Count > 0 || Type is CoreAttribute)
            {
                classRegion += ": ";
            }

            List<string> inheritedTypes = new List<string>();

            if(Type.BaseClass != null)
            {
                inheritedTypes.Add(this.BuildGenericType(Type.BaseClass));
            }
            if(Type is CoreAttribute && Type.BaseClass == null)
            {
                inheritedTypes.Add(nameof(Attribute));
            }
            if (Type.Interfaces != null)
            {
                inheritedTypes.AddRange(Type.Interfaces.Select(r => this.BuildGenericType(r)));
            }

            classRegion += String.Join(", ", inheritedTypes);

            if(Type.GenericArguments != null && Type.GenericArguments.Any(s => s.BaseClass != null || s.Interfaces != null))
            {
                List<string> genericConstraints = new List<string>();
                genericConstraints = Type.GenericArguments.Select(r => this.BuildGenericConstraint(r)).ToList();
                classRegion += $" {String.Join(" ", genericConstraints)}";
            }

            fileText.Add(classRegion);
            fileText.Add("\t{");

            if (Type.Properties != null)
            {
                List<string> properties = new List<string>();
                properties = Type.Properties.Select(r => this.BuildProperty(Type, r)).ToList();

                properties.ForEach(r =>
                {
                    fileText.Add(r);
                });
            }

            if(Type is CoreAttribute attr)
            {
                List<string> arguments = new List<string>();

                if (attr.RequiredArguments != null)
                {
                    arguments = attr.RequiredArguments.Select(r => this.BuildAttrArgument(r)).ToList();
                }

                fileText.Add($"\t\tpublic {attr.TypeName}({String.Join(", ", arguments)})");
                fileText.Add("\t\t{");
                fileText.Add("\t\t}");
            }
            else if(Type is CoreEnum Enum && Enum.Values != null)
            {
                List<string> valueStatements = new List<string>();
                Enum.Values.Keys.ToList().ForEach(r =>
                {
                    valueStatements.Add($"\t\t{r} = {Enum.Values[r]}");
                });
                fileText.Add((String.Join($",{Environment.NewLine}", valueStatements)));
            }

            fileText.Add("\t}");
            fileText.Add("}");

            string compiledText = String.Join(Environment.NewLine, fileText);

            File.WriteAllText(filePath, compiledText);
        }

        private string BuildProperty(CoreType Type, CoreProperty Property)
        {
            List<string> propertyText = new List<string>();
          
            if(Property.Attributes != null)
            {
                List<string> attrs = Property.Attributes.Select(r => $"\t\t{this.BuildAttributeInstance(r)}").ToList();
                propertyText.AddRange(attrs);
            }
            
            string modifier = "";

            if (!Type.IsInterface)
            {
                modifier = "public";
                if (Property.Overriden)
                {
                    modifier = $"{modifier} override";
                }
                else if (Property.Virtual)
                {
                    modifier = $"{modifier} virtual";
                }
            }

            if (!String.IsNullOrEmpty(modifier))
            {
                propertyText.Add($"\t\t{modifier} {this.BuildGenericType(Property.Type)} {Property.PropertyName} {{ get; set; }}");
            }
            else
            {
                propertyText.Add($"\t\t{this.BuildGenericType(Property.Type)} {Property.PropertyName} {{ get; set; }}");
            }

            return String.Join(Environment.NewLine, propertyText);
        }

        private string BuildAttributeInstance(CoreAttributeInstance Attr)
        {
            string attrInsides = Attr.TypeName;

            if(Attr.Arguments != null && Attr.Arguments.Count() > 0)
            {
                List<string> args = Attr.Arguments.Select(r =>
                {
                    if (r.Value is JsonElement elem)
                    {
                        if(elem.ValueKind == JsonValueKind.String)
                        {
                            return $"\"{elem.ToString()}\"";
                        }
                        else
                        {
                            return elem.GetRawText();
                        }
                    }
                    return "";
                }).ToList();

                attrInsides += $"({String.Join(", ", args)})";
            }

            return $"[{attrInsides}]";
        }

        private string BuildAttrArgument(CoreArgument Arg)
        {
            string argString = $"{this.GetTypeName(Arg.Type)} {Arg.ArgumentName}";

            if(Arg.DefaultValue != null && Arg.DefaultValue is JsonElement elem)
            {
                if(elem.ValueKind == JsonValueKind.String) 
                {
                    argString += $" = \"{elem.GetString()}\"";
                }
                else
                {
                    argString += $" = {elem.GetRawText()}";
                }
            }
            return argString;
        }

        private string BuildGenericConstraint(CoreType Type)
        {
            string constraint = $"where {Type.TypeName}: ";
            List<string> constraints = new List<string>();

            if(Type.BaseClass != null)
            {
                constraints.Add(this.BuildGenericType(Type.BaseClass));
            }
            if(Type.Interfaces != null)
            {
                constraints.AddRange(Type.Interfaces.Select(r => this.BuildGenericType(r)));
            }

            constraint += String.Join(", ", constraints);
            return constraint;
        }

        private List<string> ExtractImports(CoreType Type)
        {
            List<string> imports = new List<string>();

            if(Type.GenericArguments != null)
            {
                imports.AddRange(Type.GenericArguments.Select(r => r.NameSpace));
                imports.AddRange(Type.GenericArguments.SelectMany(r => this.ExtractImports(r)));
            }

            if(Type.Attributes != null)
            {
                imports.AddRange(Type.Attributes.Select(r => r.NameSpace));
            }

            if (Type.BaseClass != null)
            {
                imports.Add(Type.BaseClass.NameSpace);
                imports.AddRange(this.ExtractImports(Type.BaseClass));
            }

            if(Type.Interfaces != null)
            {
                imports.AddRange(Type.Interfaces.Select(r => r.NameSpace));
                imports.AddRange(Type.Interfaces.SelectMany(r => this.ExtractImports(r)));
            }

            if(Type.Properties != null)
            {
                imports.AddRange(Type.Properties.Select(r => r.Type.NameSpace));
                imports.AddRange(Type.Properties.SelectMany(r => this.ExtractImports(r.Type)));
                List<CoreAttributeInstance> attrs = Type.Properties.Where(r => r.Attributes != null).SelectMany(r => r.Attributes).ToList();
                imports.AddRange(attrs.Select(r => r.NameSpace));
            }

            if (Type is CoreAttribute attr && attr.RequiredArguments != null)
            {
                imports.AddRange(attr.RequiredArguments.SelectMany(r => this.ExtractImports(r.Type)));
            }

            return imports.Where(r => r != null && r != Type.NameSpace).Distinct().ToList();
        }

        protected override Dictionary<string, string> ExtractAdditionalModules(CoreModule Module)
        {
            Dictionary<string, string> additionalModules = new Dictionary<string, string>();

            Module.Types.ForEach(r =>
            {
                additionalModules.Merge(this.ExtractAdditionalModules(r));
            });

            return additionalModules;
        }

        private Dictionary<string, string> ExtractAdditionalModules(CoreType Type)
        {
            if (this.ImportCache.Contains(Type))
            {
                return new Dictionary<string, string>();
            }

            Dictionary<string, string> additionalModules = new Dictionary<string, string>();

            if (Type.GenericArguments != null)
            {
                Type.GenericArguments.Where(r => r.Module != null).ToList().ForEach(r => additionalModules[r.Module] = r.ModuleVersion);
                List<Dictionary<string, string>> otherModules = Type.GenericArguments.Select(r => this.ExtractAdditionalModules(r)).ToList();

                otherModules.ForEach(r => additionalModules.Merge(r));
            }

            if (Type.BaseClass != null)
            {
                additionalModules[Type.BaseClass.Module] = Type.BaseClass.ModuleVersion;
                additionalModules.Merge(this.ExtractAdditionalModules(Type.BaseClass));
            }

            if (Type.Interfaces != null)
            {
                Type.Interfaces.Where(r => r.Module != null).ToList().ForEach(r => additionalModules[r.Module] = r.ModuleVersion);
                List<Dictionary<string, string>> otherModules = Type.Interfaces.Select(r => this.ExtractAdditionalModules(r)).ToList();
                otherModules.ForEach(r => additionalModules.Merge(r));
            }

            if (Type.Properties != null)
            {
                Type.Properties.Where(r => r.Type.Module != null).ToList().ForEach(r => additionalModules[r.Type.Module] = r.Type.ModuleVersion);
                List<Dictionary<string, string>> otherModules = Type.Properties.Select(r => this.ExtractAdditionalModules(r.Type)).ToList();
                otherModules.ForEach(r => additionalModules.Merge(r));
            }

            if (Type is CoreAttribute attr && attr.RequiredArguments != null)
            {
                attr.RequiredArguments.Where(r => r.Type.Module != null).ToList().ForEach(r => additionalModules[r.Type.Module] = r.Type.ModuleVersion);
                List<Dictionary<string, string>> otherModules = attr.RequiredArguments.Select(r => this.ExtractAdditionalModules(r.Type)).ToList();
                otherModules.ForEach(r => additionalModules.Merge(r));
            }

            this.ImportCache.Add(Type);
            return additionalModules;
        }

        private string BuildGenericType(CoreType Type)
        {
            string type = this.GetTypeName(Type);

            if (type != "Array" && Type.GenericArguments != null && Type.GenericArguments.Count > 0)
            {
                type += "<";
                string generics = String.Join(", ", Type.GenericArguments.Select(r => this.BuildGenericType(r)));
                type += generics + ">";
            }
            else if(type == "Array")
            {
                string generics = String.Join(", ", Type.GenericArguments.Select(r => this.BuildGenericType(r)));
                type = generics + "[]";
            }

            return type;
        }

        private string GetTypeName(CoreType Type)
        {
            List<Type> types = typeof(string).Assembly.GetTypes().ToList();
            Type foundType = types.FirstOrDefault(r => r.Name == Type.TypeName);

            if (Type.IsPrimitive && foundType != null)
            {
                if (LinqlModelGeneratorCSharpFrontend.Aliases.ContainsKey(foundType))
                {
                    return LinqlModelGeneratorCSharpFrontend.Aliases[foundType];
                }

                return foundType.Name;

            }
            else
            {
                return Type.TypeName;
            }
        }
    }
}
