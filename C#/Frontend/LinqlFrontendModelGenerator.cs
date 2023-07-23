using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Linql.ModelGenerator.Intermediary;

namespace Linql.ModelGenerator.Frontend
{
    public partial class LinqlFrontendModelGenerator
    {
        public string IntermediaryJson { get; set; }

        public string ProjectPath { get; set; } 

        IntermediaryModule Module { get; set; }

        public LinqlFrontendModelGenerator(string IntermediaryJson, string ProjectPath = null) 
        {
            this.IntermediaryJson = IntermediaryJson;
            if (ProjectPath == null)
            {
                this.ProjectPath = Environment.CurrentDirectory;
            }
            else
            {
                this.ProjectPath = ProjectPath;
            }
            this.Module = JsonSerializer.Deserialize<IntermediaryModule>(this.IntermediaryJson);
        }

        public void Generate(string IntermediaryJson, string ProjectPath = null)
        {
            this.CreateProject();

            this.Module.Types.ForEach(r =>
            {
                this.CreateType(r);
            });
        }

        protected void CreateProject()
        {
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo("dotnet");
            processStartInfo.Arguments = $"new classlib -o {this.Module.ModuleName} -f netstandard2.0";
            processStartInfo.WorkingDirectory = this.ProjectPath;
            processStartInfo.CreateNoWindow = false;
            process.StartInfo = processStartInfo;
            process.Start();

            process.WaitForExit(1000);

            File.Delete(Path.Combine(this.ProjectPath, this.Module.ModuleName, "Class1.cs"));
        }

        public void Clean()
        {
            Directory.Delete(Path.Combine(this.ProjectPath, this.Module.ModuleName), true);
        }

        protected void CreateType(IntermediaryType Type)
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
            else
            {
                throw new Exception($"Unable to determine class type for Type {Type.TypeName}");
            }

            string classRegion = $"\tpublic {classType} {this.GetTypeName(Type)}";

            if(Type.IsGenericType)
            {
                classRegion += "<";
                string generics = String.Join(", ", Type.GenericArguments.Select(r => this.GetTypeName(r)));
                classRegion += generics + ">";
            }

            if(Type.BaseClass != null || Type.Interfaces?.Count > 0)
            {
                classRegion += ": ";
            }

            List<string> inheritedTypes = new List<string>();

            if(Type.BaseClass != null)
            {
                inheritedTypes.Add(this.BuildGenericType(Type.BaseClass));
            }
            if (Type.Interfaces != null)
            {
                inheritedTypes.AddRange(Type.Interfaces.Select(r => this.BuildGenericType(r)));
            }

            classRegion += String.Join(", ", inheritedTypes);

            fileText.Add(classRegion);
            fileText.Add("\t{");

            if (Type.Properties != null)
            {
                List<string> properties = new List<string>();

                if (Type.IsInterface)
                {
                    properties = Type.Properties.Select(r => $"{this.BuildGenericType(r.Type)} {r.PropertyName} {{ get; set; }}").ToList();
                }
                else
                {
                    properties = Type.Properties.Select(r => $"public {this.BuildGenericType(r.Type)} {r.PropertyName} {{ get; set; }}").ToList();
                }
                properties.ForEach(r =>
                {
                    fileText.Add($"\t\t{r}");
                    fileText.Add(Environment.NewLine);
                });
            }

            if(Type is IntermediaryAttribute attr)
            {
                List<string> arguments = new List<string>();

                if (attr.Arguments != null)
                {
                    arguments = attr.Arguments.Select(r => this.BuildAttrArgument(r)).ToList();
                }

                fileText.Add($"\t\tpublic {attr.TypeName}({String.Join(", ", arguments)})");
                fileText.Add("\t\t{");
                fileText.Add("\t\t}");
            }

            fileText.Add("\t}");
            fileText.Add("}");

            string compiledText = String.Join(Environment.NewLine, fileText);

            File.WriteAllText(filePath, compiledText);
        }

        private string BuildAttrArgument(IntermediaryArgument Arg)
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

        private List<string> ExtractImports(IntermediaryType Type)
        {
            List<string> imports = new List<string>();

            if(Type.GenericArguments != null)
            {
                imports.AddRange(Type.GenericArguments.Select(r => r.NameSpace));
                imports.AddRange(Type.GenericArguments.SelectMany(r => this.ExtractImports(r)));
            }

            if(Type.BaseClass != null)
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
            }

            if (Type is IntermediaryAttribute attr && attr.Arguments != null)
            {
                imports.AddRange(attr.Arguments.SelectMany(r => this.ExtractImports(r.Type)));
            }

            return imports.Where(r => r != null && r != Type.NameSpace).Distinct().ToList();
        }

        private string BuildGenericType(IntermediaryType Type)
        {
            string type = this.GetTypeName(Type);

            if (type != "Array" && Type.GenericArguments != null && Type.GenericArguments.Count > 0)
            {
                type += "<";
                string generics = String.Join(", ", Type.GenericArguments.Select(r => this.GetTypeName(r)));
                type += generics + ">";
            }
            else if(type == "Array")
            {
                string generics = String.Join(", ", Type.GenericArguments.Select(r => this.GetTypeName(r)));
                type = generics + "[]";
            }

            return type;
        }

        private string GetTypeName(IntermediaryType Type)
        {
            List<Type> types = typeof(string).Assembly.GetTypes().ToList();
            Type foundType = types.FirstOrDefault(r => r.Name == Type.TypeName);

            if (Type.IsPrimitive && foundType != null)
            {
                if (LinqlFrontendModelGenerator.Aliases.ContainsKey(foundType))
                {
                    return LinqlFrontendModelGenerator.Aliases[foundType];
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
