using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using Linql.ModelGenerator.Intermediary;

namespace Linql.ModelGenerator.Typescript.Frontend
{
    public partial class LinqlModelGeneratorTypescriptFrontend
    {
        public string IntermediaryJson { get; set; }

        public string ProjectPath { get; set; }

        IntermediaryModule Module { get; set; }

        public bool SkipInstall { get; set; } = true;

        private HashSet<IntermediaryType> ImportCache = new HashSet<IntermediaryType>();

        public LinqlModelGeneratorTypescriptFrontend(string IntermediaryJson, string ProjectPath = null)
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

        public void Generate()
        {
            this.CreateProject();

            Dictionary<string, string> additionalModules = this.ExtractAdditionalModules(this.Module);

            additionalModules.Remove(this.Module.ModuleName);

            this.AddAdditionalModules(additionalModules);

            this.Module.Types.ForEach(r =>
            {
                this.CreateType(r);
            });

            this.WritePublicApi();
        }

        protected void AddAdditionalModules(Dictionary<string, string> AdditionalModules)
        {
            string projectFolder = this.GetAngularLibRoot();
            string packageJsonFile = Path.Combine(projectFolder, "package.json");
            string packageJsonText = File.ReadAllText(packageJsonFile);
            JsonNode packageJson = JsonNode.Parse(packageJsonText);

            JsonNode root = packageJson.Root;
            root["version"] = this.Module.Version;

            root["peerDependencies"] = new JsonObject();
            root["devDependencies"] = new JsonObject();

            JsonNode peerDependencies = root["peerDependencies"];
            JsonNode devDependencies = root["devDependencies"];

            foreach (var dep in AdditionalModules)
            {
                peerDependencies[this.GetAngularLibraryName(dep.Key)] = dep.Value;
                devDependencies[this.GetAngularLibraryName(dep.Key)] = dep.Value;
            }

            File.WriteAllText(packageJsonFile, root.ToJsonString(new JsonSerializerOptions() { WriteIndented = true }));
        }

        protected void CreateProject()
        {
            Process ngNewProcess = new Process();
            ProcessStartInfo ngNew = new ProcessStartInfo("Powershell.exe");
            ngNew.Arguments = $"ng new {this.Module.ModuleName}";

            if (this.SkipInstall)
            {
                ngNew.Arguments += " --skip-install";
            }

            ngNew.WorkingDirectory = this.ProjectPath;
            ngNew.CreateNoWindow = false;
            ngNewProcess.StartInfo = ngNew;
            ngNewProcess.Start();

            ngNewProcess.WaitForExit();

            string libraryName = this.GetAngularLibraryName(this.Module.ModuleName);

            Process ngNewLibrary = new Process();
            ProcessStartInfo ngLibrary = new ProcessStartInfo("Powershell.exe");
            ngLibrary.Arguments = $"ng generate library {libraryName}";

            if (this.SkipInstall)
            {
                ngLibrary.Arguments += " --skip-install";
            }

            ngLibrary.WorkingDirectory = this.GetAngularAppPath();
            ngLibrary.CreateNoWindow = false;
            ngNewLibrary.StartInfo = ngLibrary;
            ngNewLibrary.Start();

            ngNewLibrary.WaitForExit();

            string srcDirectory = this.GetAngularSrcPath();
            string libDirectory = this.GetAngularLibPath();

            File.WriteAllText(Path.Combine(srcDirectory, "public-api.ts"), $"// Public API Surface of {libraryName}");
            Directory.GetFiles(libDirectory).ToList().ForEach(r => File.Delete(r));
        }

        protected void WritePublicApi()
        {
            string srcDirectory = this.GetAngularSrcPath();
            string publicApiFile = Path.Combine(srcDirectory, "public-api.ts");
            List<string> publicApiText = File.ReadAllLines(publicApiFile).ToList();

            this.Module.Types.ForEach(r =>
            {
                string folder = this.GetNamespaceDirectory(r.NameSpace);
                string directory = Path.Combine("src", "lib", folder);
                string relativePath = this.GetRelativeImport("src", directory);
                publicApiText.Add($"export * from './{relativePath}{r.TypeName}';");
            });

            File.WriteAllLines(publicApiFile, publicApiText);
        }


        public void Clean()
        {
            Directory.Delete(Path.Combine(this.ProjectPath, this.Module.ModuleName), true);
        }

        protected string GetNamespaceDirectory(string NameSpace)
        {
            string folder = NameSpace.Replace($"{this.Module.ModuleName}", String.Empty).TrimStart('.');
            return folder;
        }

        protected string GetRelativeImport(string NameSpace1, string Namespace2)
        {
            string folder1 = "C:/";
            string folder2 = folder1;

            string namespaceFolder1 = this.GetNamespaceDirectory(NameSpace1);
            string namespaceFolder2 = this.GetNamespaceDirectory(Namespace2);

            if (!String.IsNullOrEmpty(namespaceFolder1))
            {
                folder1 += namespaceFolder1 + "/";
            }
            if (!String.IsNullOrEmpty(namespaceFolder2))
            {
                folder2 += namespaceFolder2 + "/";
            }

            Uri path1 = new Uri(folder1);
            Uri path2 = new Uri(folder2);
            Uri diff = path1.MakeRelativeUri(path2);
            string relativePath = diff.OriginalString;

            if (String.IsNullOrEmpty(relativePath))
            {
                relativePath = "./";
            }

            return relativePath;
        }


        protected void CreateType(IntermediaryType Type)
        {
            List<string> fileText = new List<string>();
            string folder = this.GetNamespaceDirectory(Type.NameSpace);
            string directory = Path.Combine(this.GetAngularLibPath(), folder);
            string filePath = Path.Combine(directory, $"{Type.TypeName}.ts");
            Directory.CreateDirectory(directory);

            List<TypescriptImport> imports = this.ExtractImports(Type);
            List<TypescriptImport> localImports = imports.Where(r => !String.IsNullOrEmpty(r.ModuleName) && r.ModuleName == this.Module.ModuleName).ToList();

            List<IGrouping<string, TypescriptImport>> moduleImports = imports.Except(localImports).Where(r => !String.IsNullOrEmpty(r.ModuleName)).GroupBy(r => r.ModuleName).ToList();

            List<string> importStatements = localImports.Select(r => $"import {{ {r.TypeName} }} from '{Path.Combine(this.GetRelativeImport(Type.NameSpace, r.NameSpace), r.TypeName)}';").Distinct().ToList();

            moduleImports.ForEach(r =>
            {
                List<string> typesToImport = r.Select(s => s.TypeName).Distinct().ToList();
                string typeImportString = String.Join(", ", typesToImport);
                importStatements.Add($"import {{ {typeImportString} }} from '{this.GetAngularLibraryName(r.Key)}';");
            });

            importStatements = importStatements.Select(r => r.Replace("\\", "/")).ToList();

            fileText.AddRange(importStatements);

            if(Type is IntermediaryAttribute)
            {
                fileText.Add("import 'reflect-metadata';");
            }

            fileText.Add("");

            if (Type is IntermediaryAttribute attr)
            {
                string attributeSymbol = $"{attr.TypeName}AttributeKey";
                fileText.Add($"export const {attributeSymbol}: Symbol = Symbol('{attr.TypeName}')");
                fileText.Add("");

                List<string> arguments = new List<string>();
                List<string> metadataArguments = new List<string>();
                if(attr.Arguments != null)
                {
                    arguments = attr.Arguments.Select(r => this.BuildAttrArgument(r)).ToList();
                    metadataArguments = attr.Arguments.Select(r => r.ArgumentName).ToList();
                }

                fileText.Add($"export function {attr.TypeName}({String.Join(", ", arguments)})");
                fileText.Add("{");
                fileText.Add($"\treturn Reflect.metadata({attributeSymbol}, [{String.Join(", ", metadataArguments)}]);");
                fileText.Add("}");
                fileText.Add("");
                fileText.Add($"export function get{attr.TypeName}(target: any, propertyKey: string)");
                fileText.Add("{");
                fileText.Add($"\treturn Reflect.getMetadata({attributeSymbol}, target, propertyKey);");
                fileText.Add("}");

            }
            else
            {
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

                //if (Type.Attributes != null)
                //{
                //    List<string> attrs = Type.Attributes.Select(r => $"{this.BuildAttributeInstance(r)}").ToList();
                //    fileText.AddRange(attrs);
                //}

                string classRegion = $"export {classType} {this.GetTypeName(Type)}";

                if (Type.IsGenericType)
                {
                    classRegion += "<";
                    string generics = String.Join(", ", Type.GenericArguments.Select(r => this.GetGenericArgumentDefinition(r)));
                    classRegion += generics + ">";
                }

                if (Type.BaseClass != null || Type.Interfaces?.Count > 0)
                {
                    classRegion += " ";
                }

                List<string> inheritedTypes = new List<string>();

                if (Type.BaseClass != null)
                {
                    inheritedTypes.Add($"extends {this.BuildGenericType(Type.BaseClass)}");
                }
                //if(Type is IntermediaryAttribute)
                //{
                //    inheritedTypes.Add("Attribute");
                //}
                if (Type.Interfaces != null)
                {
                    string modifier = "implements";
                    if (Type.IsInterface)
                    {
                        modifier = "extends";
                    }
                    inheritedTypes.Add($"{modifier} {String.Join(", ", Type.Interfaces.Select(r => this.BuildGenericType(r)))}");
                }

                classRegion += String.Join(" ", inheritedTypes);

                //if(Type.GenericArguments != null && Type.GenericArguments.Any(s => s.BaseClass != null || s.Interfaces != null))
                //{
                //    List<string> genericConstraints = new List<string>();
                //    genericConstraints = Type.GenericArguments.Select(r => this.BuildGenericConstraint(r)).ToList();
                //    classRegion += $" {String.Join(" ", genericConstraints)}";
                //}

                fileText.Add(classRegion);
                fileText.Add("{");

                if (Type.Properties != null)
                {
                    List<string> properties = new List<string>();

                    if (Type.IsInterface)
                    {
                        properties = Type.Properties.Select(r => this.BuildProperty(r)).ToList();
                    }
                    else
                    {
                        properties = Type.Properties.Select(r => this.BuildProperty(r, "public")).ToList();
                    }

                    properties.ForEach(r =>
                    {
                        fileText.Add(r);
                        //fileText.Add(Environment.NewLine);
                    });
                }

                //if(Type is IntermediaryAttribute attr)
                //{
                //    List<string> arguments = new List<string>();

                //    if (attr.Arguments != null)
                //    {
                //        arguments = attr.Arguments.Select(r => this.BuildAttrArgument(r)).ToList();
                //    }

                //    fileText.Add($"\t\tpublic {attr.TypeName}({String.Join(", ", arguments)})");
                //    fileText.Add("\t\t{");
                //    fileText.Add("\t\t}");
                //}

                fileText.Add("}");
            }

            string compiledText = String.Join(Environment.NewLine, fileText);

            File.WriteAllText(filePath, compiledText);
        }

        private string BuildProperty(IntermediaryProperty Property, string Modifier = null)
        {
            List<string> propertyText = new List<string>();

            if (Property.Attributes != null)
            {
                List<string> attrs = Property.Attributes.Select(r => $"\t{this.BuildAttributeInstance(r)}").ToList();
                propertyText.AddRange(attrs);
            }

            if (!String.IsNullOrEmpty(Modifier))
            {
                propertyText.Add($"\t{Modifier} {Property.PropertyName}!: {this.BuildGenericType(Property.Type)};");
            }
            else
            {
                propertyText.Add($"\t{Property.PropertyName}: {this.BuildGenericType(Property.Type)};");
            }

            return String.Join(Environment.NewLine, propertyText);
        }

        private string BuildAttributeInstance(IntermediaryAttributeInstance Attr)
        {
            string attrInsides = Attr.TypeName;
            List<string> args = new List<string>();
            if (Attr.Arguments != null && Attr.Arguments.Count() > 0)
            {
                args = Attr.Arguments.Select(r =>
                {
                    if (r.Value is JsonElement elem)
                    {
                        if (elem.ValueKind == JsonValueKind.String)
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

            }
            attrInsides += $"({String.Join(", ", args)})";
            return $"@{attrInsides}";
        }

        private string BuildAttrArgument(IntermediaryArgument Arg)
        {
            string argString = $"{Arg.ArgumentName}: {this.GetTypeName(Arg.Type)}";

            if (Arg.DefaultValue != null && Arg.DefaultValue is JsonElement elem)
            {
                if (elem.ValueKind == JsonValueKind.String)
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

        private string BuildGenericConstraint(IntermediaryType Type)
        {
            string constraint = $"where {Type.TypeName}: ";
            List<string> constraints = new List<string>();

            if (Type.BaseClass != null)
            {
                constraints.Add(this.BuildGenericType(Type.BaseClass));
            }
            if (Type.Interfaces != null)
            {
                constraints.AddRange(Type.Interfaces.Select(r => this.BuildGenericType(r)));
            }

            constraint += String.Join(", ", constraints);
            return constraint;
        }

        private List<TypescriptImport> ExtractImports(IntermediaryType Type)
        {
            List<TypescriptImport> imports = new List<TypescriptImport>();

            if (Type.GenericArguments != null)
            {

                imports.AddRange(Type.GenericArguments.Select(r => new TypescriptImport(r.TypeName, r.Module, r.NameSpace)));
                imports.AddRange(Type.GenericArguments.SelectMany(r => this.ExtractImports(r)));
            }

            if (Type.Attributes != null)
            {
                imports.AddRange(Type.Attributes.Select(r => new TypescriptImport(r.TypeName, r.Module, r.NameSpace)));
            }

            if (Type.BaseClass != null)
            {
                imports.Add(new TypescriptImport(Type.BaseClass.TypeName, Type.BaseClass.Module, Type.BaseClass.NameSpace));
                imports.AddRange(this.ExtractImports(Type.BaseClass));
            }

            if (Type.Interfaces != null)
            {
                imports.AddRange(Type.Interfaces.Select(r => new TypescriptImport(r.TypeName, r.Module, r.NameSpace)));
                imports.AddRange(Type.Interfaces.SelectMany(r => this.ExtractImports(r)));
            }

            if (Type.Properties != null)
            {
                imports.AddRange(Type.Properties.Select(r => new TypescriptImport(r.Type.TypeName, r.Type.Module, r.Type.NameSpace)));
                imports.AddRange(Type.Properties.SelectMany(r => this.ExtractImports(r.Type)));
                List<IntermediaryAttributeInstance> attrs = Type.Properties.Where(r => r.Attributes != null).SelectMany(r => r.Attributes).ToList();
                imports.AddRange(attrs.Select(r => new TypescriptImport(r.TypeName, r.Module, r.NameSpace)));
            }

            if (Type is IntermediaryAttribute attr && attr.Arguments != null)
            {
                imports.AddRange(attr.Arguments.SelectMany(r => this.ExtractImports(r.Type)));
            }

            return imports;
        }

        private Dictionary<string, string> ExtractAdditionalModules(IntermediaryModule Module)
        {
            Dictionary<string, string> additionalModules = new Dictionary<string, string>();

            Module.Types.ForEach(r =>
            {
                additionalModules.Merge(this.ExtractAdditionalModules(r));
            });

            return additionalModules;
        }

        private Dictionary<string, string> ExtractAdditionalModules(IntermediaryType Type)
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

            if (Type is IntermediaryAttribute attr && attr.Arguments != null)
            {
                attr.Arguments.Where(r => r.Type.Module != null).ToList().ForEach(r => additionalModules[r.Type.Module] = r.Type.ModuleVersion);
                List<Dictionary<string, string>> otherModules = attr.Arguments.Select(r => this.ExtractAdditionalModules(r.Type)).ToList();
                otherModules.ForEach(r => additionalModules.Merge(r));
            }

            this.ImportCache.Add(Type);
            return additionalModules;
        }

        private string BuildGenericType(IntermediaryType Type)
        {
            string type = this.GetTypeName(Type);


            if (type != "Dictionary" && Type.GenericArguments != null && Type.GenericArguments.Count > 0)
            {
                type += "<";
                string generics = String.Join(", ", Type.GenericArguments.Select(r => this.GetTypeName(r)));
                type += generics + ">";
            }
            else if (type == "Dictionary")
            {
                List<string> dictionaryTypes = Type.GenericArguments.Select(r => this.GetTypeName(r)).ToList();
                type = $"{{ [key: {dictionaryTypes[0]}]: {dictionaryTypes[1]} }}";
            }

            return type;
        }

        private string GetGenericArgumentDefinition(IntermediaryType Type)
        {
            string typeName = this.GetTypeName(Type);
            List<string> inheritedTypes = new List<string>();

            if (Type.BaseClass != null || Type.Interfaces != null)
            {
                typeName += " ";
            }

            if (Type.BaseClass != null)
            {
                inheritedTypes.Add($"extends {this.BuildGenericType(Type.BaseClass)}");
            }
            if (Type.Interfaces != null)
            {
                string modifier = "extends";
                inheritedTypes.Add($"{modifier} {String.Join(", ", Type.Interfaces.Select(r => this.BuildGenericType(r)))}");
            }

            typeName += String.Join(" ", inheritedTypes);
            return typeName;
        }

        private string GetTypeName(IntermediaryType Type)
        {
            List<string> arrayTypes = new List<string>() { "List", "Array" };
            List<Type> types = typeof(string).Assembly.GetTypes().ToList();
            Type foundType = types.FirstOrDefault(r => r.Name == Type.TypeName);

            if (Type.IsPrimitive && foundType != null)
            {
                if (LinqlModelGeneratorTypescriptFrontend.Aliases.ContainsKey(foundType))
                {
                    return LinqlModelGeneratorTypescriptFrontend.Aliases[foundType];
                }

                return foundType.Name;

            }
            else if (arrayTypes.Contains(Type.TypeName))
            {
                return "Array";
            }
            else
            {
                return Type.TypeName;
            }
        }
    }
}
