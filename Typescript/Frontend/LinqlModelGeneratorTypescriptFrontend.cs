using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using Linql.ModelGenerator.Core;

namespace Linql.ModelGenerator.Typescript.Frontend
{
    public partial class LinqlModelGeneratorTypescriptFrontend : LinqlFrontendModelGenerator
    {
        public bool SkipInstall { get; set; } = true;

        public bool InitializeProperties { get; set; } = true;

        public List<CoreType> AnyCasts = new List<CoreType>();

        public List<TypescriptGeneratorPlugin> Plugins { get; set; } = new List<TypescriptGeneratorPlugin>();

        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        private string Powershell 
        { 
            get 
            { 
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Powershell.exe" : "pwsh";
            } 
        }

        public LinqlModelGeneratorTypescriptFrontend(string CoreJson, List<TypescriptGeneratorPlugin> Plugins = null, string ProjectPath = null) : base(CoreJson, ProjectPath) 
        {
            if (Plugins != null)
            {
                this.Plugins = Plugins;
            }
        }
        public LinqlModelGeneratorTypescriptFrontend(CoreModule Module, List<TypescriptGeneratorPlugin> Plugins = null, string ProjectPath = null) : base(Module, ProjectPath) 
        {
            if(Plugins != null)
            {
                this.Plugins = Plugins;
            }
        }


        public override void Generate()
        {
            //Generic names in typescript can't have duplicate names.
            this.Module.Types.GroupBy(r => new { r.TypeName, r.NameSpace }).Where(r => r.Count() > 1).ToList().ForEach(r =>
            {
                r.ToList().ForEach(s =>
                {
                    if (s.GenericArguments != null)
                    {
                        s.TypeName = $"{s.TypeName}{s.GenericArguments.Count}";
                    }
                    else
                    {
                        s.TypeName = $"{s.TypeName}";
                    }
                });
            });

            base.Generate();
            this.WritePublicApi();
        }

        protected void ReplaceAnyOverrides(CoreType Type)
        {
            bool isAny = this.IsAnyType(Type);

            if (isAny == true)
            {
                Type.Module = null;
                Type.TypeName = "object";
                Type.IsPrimitive = true;
                Type.NameSpace = null;
            }
            else if (Type.GenericArguments != null)
            {
                Type.GenericArguments.ForEach(r => this.ReplaceAnyOverrides(r));
            }
        }

        protected override void AddAdditionalModules(Dictionary<string, string> AdditionalModules)
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

            peerDependencies["reflect-metadata"] = "*";

            foreach (var dep in AdditionalModules)
            {
                string version = dep.Value;
                //version = String.Join(".", version.Split('.').Take(2));
                //version += ".*-*";
                peerDependencies[this.GetAngularLibraryName(dep.Key)] = $"^{version}";
                devDependencies[this.GetAngularLibraryName(dep.Key)] = $"^{version}";
            }

            File.WriteAllText(packageJsonFile, root.ToJsonString(new JsonSerializerOptions() { WriteIndented = true }));
        }

        protected Process GetProcess(string Process = null)
        {
            if (String.IsNullOrEmpty(Process))
            {
                Process = this.Powershell;
            }
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo(Process);
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo = processStartInfo;
            process.OutputDataReceived += new DataReceivedEventHandler
            (
                delegate (object sender, DataReceivedEventArgs e)
                {
                    Console.WriteLine(e.Data);
                }
            );
            process.ErrorDataReceived += new DataReceivedEventHandler
            (
                delegate (object sender, DataReceivedEventArgs e)
                {
                    Console.WriteLine(e.Data);
                }
            );
            return process;
        }

        protected void RunProcess(Process Process)
        {
            Process.Start();
            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();
            Process.WaitForExit();
            Process.CancelOutputRead();
            Process.CancelErrorRead();

        }

        protected string GetProcessArgs(string Command)
        {
            if(IsWindows)
            {
                return Command;
            }

            return $"-Command {Command}";
        }

        protected override void CreateProject()
        {
            Console.WriteLine($"Creating new Angular App: {this.Module.ModuleName}");
            Process ngNewProcess = this.GetProcess();
            ngNewProcess.StartInfo.Arguments = this.GetProcessArgs($"ng new {this.Module.ModuleName}");

            if (this.SkipInstall)
            {
                ngNewProcess.StartInfo.Arguments += " --skip-install";
            }

            ngNewProcess.StartInfo.Arguments += " --interactive=false --force";

            ngNewProcess.StartInfo.WorkingDirectory = this.ProjectPath;
            this.RunProcess(ngNewProcess);

            Console.WriteLine("Finished creating angular app");
            string libraryName = this.GetAngularLibraryName(this.Module.ModuleName);

            Console.WriteLine($"Generating library {libraryName}");

            Process ngNewLibrary = this.GetProcess();
            ngNewLibrary.StartInfo.Arguments = this.GetProcessArgs($"ng generate library {libraryName}");

            if (this.SkipInstall)
            {
                ngNewLibrary.StartInfo.Arguments += " --skip-install";
            }

            ngNewLibrary.StartInfo.Arguments += " --interactive=false --force";

            ngNewLibrary.StartInfo.WorkingDirectory = this.GetAngularAppPath();
            this.RunProcess(ngNewLibrary);


            Console.WriteLine("Finished creating angular library");

            string srcDirectory = this.GetAngularSrcPath();
            string libDirectory = this.GetAngularLibPath();

            File.WriteAllText(Path.Combine(srcDirectory, "public-api.ts"), $"// Public API Surface of {libraryName}");
            Directory.GetFiles(libDirectory).ToList().ForEach(r => File.Delete(r));

            string rootFolder = this.GetAngularAppPath();
            string projectFolder = this.GetAngularLibPath();
            string packageJsonFile = Path.Combine(rootFolder, "package.json");
            string packageJsonText = File.ReadAllText(packageJsonFile);

            JsonNode packageJson = JsonNode.Parse(packageJsonText);

            JsonNode root = packageJson.Root;

            string distFolder = Path.Combine(rootFolder, "dist", root["name"].ToString());

            root["scripts"]["linqlBuild"] = new JsonObject();
            root["scripts"]["linqlBuild"] = $"npm i && cd {this.GetRelativePath(rootFolder, projectFolder)} && npm i && cd ../../ && ng build {libraryName}";

            root["scripts"]["linqlPublish"] = new JsonObject();
            root["scripts"]["linqlPublish"] = $"cd dist/{libraryName} && npm publish";

            File.WriteAllText(packageJsonFile, root.ToJsonString(new JsonSerializerOptions() { WriteIndented = true }));

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

        protected string GetNamespaceDirectory(string NameSpace)
        {
            string folder = NameSpace.Replace($"{this.Module.ModuleName}", String.Empty).TrimStart('.');
            return folder;
        }

        protected string GetRelativePath(string ComparePath, string Path)
        {
            Uri path1 = new Uri(ComparePath);
            Uri path2 = new Uri(Path);
            Uri diff = path1.MakeRelativeUri(path2);
            string relativePath = diff.OriginalString;
            return "./" + String.Join("/", relativePath.Split('/').Skip(1));
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
            else if (!relativePath.StartsWith("./"))
            {
                relativePath = $"./{relativePath}";
            }

            return relativePath;
        }


        protected override void CreateType(CoreType Type)
        {
            List<string> fileText = new List<string>();
            string folder = this.GetNamespaceDirectory(Type.NameSpace);
            string directory = Path.Combine(this.GetAngularLibPath(), folder);
            string filePath = Path.Combine(directory, $"{Type.TypeName}.ts");
            Directory.CreateDirectory(directory);

            List<TypescriptImport> imports = this.ExtractImports(Type);
            List<TypescriptImport> localImports = imports.Where(r => !String.IsNullOrEmpty(r.ModuleName) && r.ModuleName == this.Module.ModuleName).ToList();

            List<IGrouping<string, TypescriptImport>> moduleImports = imports.Except(localImports).Where(r => !String.IsNullOrEmpty(r.ModuleName)).GroupBy(r => r.ModuleName).ToList();

            List<IGrouping<string, TypescriptImport>> localImportsMapping = localImports.GroupBy(r => Path.Combine(this.GetRelativeImport(Type.NameSpace, r.NameSpace), r.TypeName)).ToList();

            List<string> importStatements = localImportsMapping.Select(r => $"import {{ {String.Join(", ", r.Select(s => $"{s.TypeName}{s.AttributeSuffix}").Distinct())} }} from '{r.Key}';").Distinct().ToList();

            moduleImports.ForEach(r =>
            {
                List<string> typesToImport = r.Select(s => $"{s.TypeName}{s.AttributeSuffix}").Distinct().ToList();
                string typeImportString = String.Join(", ", typesToImport);
                importStatements.Add($"import {{ {typeImportString} }} from '{this.GetAngularLibraryName(r.Key)}';");
            });

            importStatements = importStatements.Select(r => r.Replace("\\", "/")).ToList();

            fileText.AddRange(importStatements);

            if (Type is CoreAttribute)
            {
                fileText.Add("import 'reflect-metadata';");
            }

            fileText.Add("");

            if (Type is CoreAttribute attr)
            {
                this.BuildAttributeClass(attr, fileText);
            }
            else
            {
                this.BuildTypeClass(Type, fileText);
            }

            string compiledText = String.Join(Environment.NewLine, fileText);

            File.WriteAllText(filePath, compiledText);
        }

        private void BuildTypeClass(CoreType Type, List<string> fileText)
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
            else if (Type is CoreEnum Enum)
            {
                classType = "enum";
            }
            else
            {
                throw new Exception($"Unable to determine class type for Type {Type.TypeName}");
            }

            if (Type.Attributes != null && Type.IsAbstract == false)
            {
                List<string> attrs = Type.Attributes.Select(r => $"{this.BuildAttributeInstance(r, "Class")}").ToList();
                fileText.AddRange(attrs);
            }

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

            fileText.Add(classRegion);
            fileText.Add("{");

            this.Plugins.ForEach(r =>
            {
                fileText.AddRange(r.BeforePropertiesBuilt(Type, this));
            });

            if (Type.Properties != null)
            {
                List<string> properties = new List<string>();

                if (Type.IsInterface)
                {
                    properties = Type.Properties.Select(r => this.BuildProperty(Type, r)).ToList();
                }
                else
                {
                    properties = Type.Properties.Select(r => this.BuildProperty(Type, r)).ToList();
                }

                properties.ForEach(r =>
                {
                    fileText.Add(r);
                });
            }

            else if (Type is CoreEnum Enum && Enum.Values != null)
            {
                List<string> valueStatements = new List<string>();
                Enum.Values.Keys.ToList().ForEach(r =>
                {
                    valueStatements.Add($"\t{r} = {Enum.Values[r]}");
                });
                fileText.Add((String.Join($",{Environment.NewLine}", valueStatements)));
            }

            fileText.Add("}");
        }

        private void BuildAttributeClass(CoreAttribute attr, List<string> fileText)
        {
            fileText.Add("type constructorType = { new(...args: any[]): {} };");
            fileText.Add("");

            this.BuildTypeClass(attr, fileText);
            bool isClassAttribute = this.IsClassAttribute(attr);
            bool isPropertyAttribute = this.IsPropertyAttribute(attr);
            fileText.Add("");

            string attributeSymbol = $"{attr.TypeName}AttributeKey";

            if (isPropertyAttribute)
            {
                fileText.Add($"export const {attributeSymbol}: Symbol = Symbol('{attr.TypeName}')");
                fileText.Add("");
            }

            if (isClassAttribute)
            {
                fileText.Add($"export function {attr.TypeName}Class(attrInstance: {attr.TypeName})");
                fileText.Add("{");

                //List<string> classArgs = this.BuildMetadataAttributeInstance(attr);
                //fileText.AddRange(classArgs);

                fileText.Add($"\treturn function {attr.TypeName}Class<T extends constructorType>(constructor: T)");
                fileText.Add("\t{");
                fileText.Add($"\t\tconstructor.prototype.{attr.TypeName} = attrInstance;");
                fileText.Add("\t}");
                fileText.Add("}");
                fileText.Add("");
            }

            if (isPropertyAttribute)
            {
                List<string> arguments = new List<string>();
                List<string> metadataArguments = new List<string>();
                if (attr.RequiredArguments != null)
                {
                    arguments = attr.RequiredArguments.Select(r => this.BuildAttrArgument(r)).ToList();
                }

                fileText.Add($"export function {attr.TypeName}Prop(attrInstance: {attr.TypeName})");
                fileText.Add("{");
                //List<string> classArgs = this.BuildMetadataAttributeInstance(attr);
                //fileText.AddRange(classArgs);
                fileText.Add($"\treturn Reflect.metadata({attributeSymbol}, attrInstance);");
                fileText.Add("}");
                fileText.Add("");
            }

            if (isPropertyAttribute && isClassAttribute)
            {
                fileText.Add($"export function get{attr.TypeName}<T extends object | constructorType, Property extends keyof T & string>(target: T, propertyKey?: Property)");
                fileText.Add("{");
                fileText.Add("\tif((target as constructorType).prototype && !propertyKey)");
                fileText.Add("\t{");
                fileText.Add($"\t\treturn (target as constructorType).prototype.{attr.TypeName} as {attr.TypeName};");
                fileText.Add("\t}");
                fileText.Add("\telse if(propertyKey)");
                fileText.Add("\t{");
                fileText.Add($"\t\treturn Reflect.getMetadata({attributeSymbol}, target, propertyKey) as {attr.TypeName};");
                fileText.Add("\t}");
                fileText.Add("\treturn null;");
                fileText.Add("}");
            }
            else if (isClassAttribute)
            {
                fileText.Add($"export function get{attr.TypeName}<T extends constructorType>(constructor: constructorType)");
                fileText.Add("{");
                fileText.Add($"\treturn constructor.prototype.{attr.TypeName} as {attr.TypeName};");
                fileText.Add("}");

            }
            else if (isPropertyAttribute)
            {
                fileText.Add($"export function get{attr.TypeName}<T extends object, Property extends keyof T & string>(target: T, propertyKey: Property)");
                fileText.Add("{");
                fileText.Add($"\treturn Reflect.getMetadata({attributeSymbol}, target, propertyKey) as {attr.TypeName};");
                fileText.Add("}");
            }
        }

        private string BuildProperty(CoreType Type, CoreProperty Property)
        {
            List<string> propertyText = new List<string>();

            if (Property.Attributes != null)
            {
                List<string> attrs = Property.Attributes.Select(r => $"\t{this.BuildAttributeInstance(r, "Prop")}").ToList();
                propertyText.AddRange(attrs);
            }

            string modifier = "";
            string propertyModifier = "";

            if (Type is CoreAttribute attr && attr.OptionalArguments != null)
            {
                if (attr.OptionalArguments.Any(s => s.ArgumentName.ToLower() == Property.PropertyName.ToLower()))
                {
                    propertyModifier = "?";
                }
            }
            if(Property.Nullable == true)
            {
                propertyModifier = "?";
            }

            if (!Type.IsInterface)
            {
                modifier = "public";
                if (Property.Overriden)
                {
                    modifier = $"{modifier} override";
                }
            }

            string propType = this.BuildGenericType(Property.Type);

            if (Property.Type.IsPrimitive == false && Property.Type.TypeName != "List")
            {
                propType += " | null";
            }

            if (!String.IsNullOrEmpty(modifier))
            {
               
                string propDef = $"\t{modifier} {Property.PropertyName}{propertyModifier}: {propType}";

                if (this.InitializeProperties && Type.IsClass)
                {
                    propDef += $" = {this.GetPropertyDefault(Property)}";
                }

                propDef += ";";

                propertyText.Add(propDef);
            }
            else
            {
                propertyText.Add($"\t{Property.PropertyName}{propertyModifier}: {propType};");
            }

            return String.Join(Environment.NewLine, propertyText);
        }

        public string GetPropertyDefault(CoreProperty Property)
        {
            string defaultProperty = "";

            if (Property.Type.IsPrimitive)
            {
                List<Type> types = typeof(string).Assembly.GetTypes().ToList();
                Type foundType = types.FirstOrDefault(r => r.Name == Property.Type.TypeName);

                if (foundType != null && LinqlModelGeneratorTypescriptFrontend.DefaultValues.ContainsKey(foundType))
                {
                    defaultProperty = LinqlModelGeneratorTypescriptFrontend.DefaultValues[foundType];
                }
                else
                {
                    defaultProperty = "null";
                }

            }
            else if(Property.Type.TypeName == "List")
            {
                string propType = this.BuildGenericType(Property.Type);
                defaultProperty = $"new {propType}()";
            }
            else
            {
                defaultProperty = "null";
            }

            return defaultProperty;
        }

        private string BuildAttributeInstance(CoreAttributeInstance Attr, string Suffix = "")
        {
            string attrInsides = $"{Attr.TypeName}{Suffix}";
            string argMap = "";
            List<string> args = new List<string>();

            if (Attr.Arguments != null && Attr.Arguments.Count() > 0)
            {
                foreach (var key in Attr.Arguments)
                {
                    object value = key.Value;
                    string stringValue = "";

                    if (value is JsonElement elem)
                    {
                        if (elem.ValueKind == JsonValueKind.String)
                        {
                            stringValue = $"\"{elem.ToString()}\"";
                        }
                        else
                        {
                            stringValue = elem.GetRawText();
                        }
                    }

                    args.Add($"{key.Key}: {stringValue}");
                }
            }
            argMap = $"{{{String.Join(", ", args)}}}";
            attrInsides += $"({argMap})";
            return $"@{attrInsides}";
        }

        private string BuildAttrArgument(CoreArgument Arg)
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


        private List<TypescriptImport> ExtractImports(CoreType Type)
        {
            List<TypescriptImport> imports = new List<TypescriptImport>();

            if (Type.GenericArguments != null)
            {

                imports.AddRange(Type.GenericArguments.Select(r => new TypescriptImport(r.TypeName, r.Module, r.NameSpace)));
                imports.AddRange(Type.GenericArguments.SelectMany(r => this.ExtractImports(r)));
            }

            if (Type.Attributes != null)
            {
                imports.AddRange(Type.Attributes.Select(r => new TypescriptImport(r.TypeName, r.Module, r.NameSpace, "Class")));
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
                List<CoreAttributeInstance> attrs = Type.Properties.Where(r => r.Attributes != null).SelectMany(r => r.Attributes).ToList();
                imports.AddRange(attrs.Select(r => new TypescriptImport(r.TypeName, r.Module, r.NameSpace, "Prop")));
            }

            if (Type is CoreAttribute attr && attr.RequiredArguments != null)
            {
                imports.AddRange(attr.RequiredArguments.SelectMany(r => this.ExtractImports(r.Type)));
            }

            return imports;
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

                List<CoreAttributeInstance> propAttributes = Type.Properties.Where(r => r.Attributes != null).SelectMany(r => r.Attributes).ToList();

                propAttributes.ForEach(r => additionalModules[r.Module] = r.ModuleVersion);
            }

            if (Type.Attributes != null)
            {
                Type.Attributes.Where(r => r.Module != null).ToList().ForEach(r => additionalModules[r.Module] = r.ModuleVersion);
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

        public string BuildGenericType(CoreType Type)
        {
            string type = this.GetTypeName(Type);

            if (type != "Dictionary" && Type.GenericArguments != null && Type.GenericArguments.Count > 0)
            {
                type += "<";
                string generics = String.Join(", ", Type.GenericArguments.Select(r => this.BuildGenericType(r)));
                type += generics + ">";
            }
            else if (type == "Dictionary")
            {
                List<string> dictionaryTypes = Type.GenericArguments.Select(r => this.BuildGenericType(r)).ToList();
                type = $"{{ [key: {dictionaryTypes[0]}]: {dictionaryTypes[1]} }}";
            }

            return type;
        }

        public string GetGenericArgumentDefinition(CoreType Type)
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

        public bool IsListType(CoreType Type)
        {
            List<string> arrayTypes = new List<string>() { "List", "Array" };
            return arrayTypes.Contains(Type.TypeName);
        }

        public string GetTypeName(CoreType Type)
        {
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
            else if (Type.TypeName == "object")
            {
                return "any";
            }
            else if (this.IsListType(Type))
            {
                return "Array";
            }
            else
            {
                return Type.TypeName;
            }
        }

        private bool IsAnyType(CoreType Type)
        {
            return this.AnyCasts.Any(s => s.TypeName == Type.TypeName && s.Module == Type.Module);
        }

    }
}
