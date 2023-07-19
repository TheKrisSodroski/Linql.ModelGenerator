using Linql.ModelGenerator.Intermediary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Linql.ModelGenerator.Backend
{
    public class LinqlModelGenerator
    {
        public Assembly Assembly { get; set; }

        protected Dictionary<Type, IntermediaryType> TypeProcessing { get; set; } = new Dictionary<Type, IntermediaryType>();

        protected List<IIgnoreTypePlugin> IgnoreTypePlugins { get; set; } = new List<IIgnoreTypePlugin>();

        protected List<IPrimitiveTypePlugin> PrimitiveTypePlugins { get; set; } = new List<IPrimitiveTypePlugin>();

        public LinqlModelGenerator(
            Assembly Assembly
            )
        {
            this.Assembly = Assembly;
            this.IgnoreTypePlugins.Add(new DefaultIgnoreTypePlugin());
            this.PrimitiveTypePlugins.Add(new DefaultPrimitiveTypePlugin());
        }

        public LinqlModelGenerator(string AssemblyPath)
        {
            string assemblyPath = AssemblyPath;
            if (!AssemblyPath.EndsWith(".dll"))
            {
                List<string> files = Directory.GetFiles(AssemblyPath).ToList();
                string csProj = files.FirstOrDefault(r => r.EndsWith("csproj"));

                if (csProj != null)
                {
                    string moduleName = Path.GetFileNameWithoutExtension(csProj);
                    Process process = new Process();
                    ProcessStartInfo processStartInfo = new ProcessStartInfo("dotnet");
                    processStartInfo.Arguments = "publish";
                    processStartInfo.WorkingDirectory = AssemblyPath;
                    processStartInfo.CreateNoWindow = false;
                    process.StartInfo = processStartInfo;
                    process.Start();

                    string compiledPath = Path.Combine(assemblyPath, "bin", "Debug");
                    List<string> compiledDirectories = Directory.GetDirectories(compiledPath).ToList();

                    string compiledDirectory = compiledDirectories.FirstOrDefault(r => r.Contains("netstandard"));

                    if (compiledDirectory == null)
                    {
                        compiledDirectory = compiledDirectories.FirstOrDefault();
                    }

                    assemblyPath = Path.Combine(compiledDirectory, $"{moduleName}.dll");
                }
                else
                {
                    throw new FileNotFoundException($"Unable to find csproj file in directory {AssemblyPath}");
                }

            }

            string fullPath = Path.GetFullPath(assemblyPath);
            this.Assembly = Assembly.LoadFile(fullPath);
            this.IgnoreTypePlugins.Add(new DefaultIgnoreTypePlugin());
            this.PrimitiveTypePlugins.Add(new DefaultPrimitiveTypePlugin());

        }

        public IntermediaryModule Generate()
        {
            IntermediaryModule module = new IntermediaryModule();
            module.BaseLanguage = "C#";
            module.ModuleName = Assembly.GetName().Name;
            module.Types = Assembly.GetTypes()
                .Where(r => !this.IgnoreTypePlugins.Any(s => s.IgnoreType(r)))
                .Select(r => this.GenerateType(r)).ToList();
            return module;
        }

        protected IntermediaryType GenerateType(Type Type)
        {
            string TypeName = Type.Name;
            if (!this.TypeProcessing.ContainsKey(Type))
            {
                IntermediaryType type;
                if (this.IsAttribute(Type))
                {
                    type = new IntermediaryAttribute();
                }
                else
                {
                    type = new IntermediaryType();
                }
                this.TypeProcessing.Add(Type, type);

                type.IsGenericType = Type.IsGenericType;
                type.IsClass = Type.IsClass;
                type.IsInterface = Type.IsInterface;
                type.IsAbstract = Type.IsAbstract;
                type.NameSpace = Type.Namespace;
                type.Module = Type.Assembly.GetName().Name;

                if (Type.BaseType != null && !this.IgnoreTypePlugins.Any(s => s.IgnoreType(Type.BaseType)))
                {
                    type.BaseClass = this.GenerateType(Type.BaseType);
                }

                List<Type> interfaces = Type.GetInterfaces().Where(r => !this.IgnoreTypePlugins.Any(s => s.IgnoreInterface(r))).ToList();
                List<Type> baseTypeInterfaces = new List<Type>();
                if (Type.BaseType != null)
                {
                    baseTypeInterfaces.AddRange(Type.BaseType.GetInterfaces().Where(r => !this.IgnoreTypePlugins.Any(s => s.IgnoreInterface(r))));
                }

                interfaces = interfaces
                    .Except(baseTypeInterfaces)
                    .Except(interfaces.SelectMany(s => s.GetInterfaces())).ToList();
                type.Interfaces = interfaces.Select(r => this.GenerateType(r)).ToList();


                if (this.IsPrimitive(Type) || this.PrimitiveTypePlugins.Any(s => s.IsPrimitiveType(Type)))
                {
                    type.NameSpace = null;
                    type.IsPrimitive = true;
                    type.TypeName = this.GetPrimitiveTypeName(Type);
                    type.BaseClass = null;
                    type.Module = null;
                }
                else
                {
                    if (this.IsArray(Type))
                    {
                        type.TypeName = "Array";
                        type.NameSpace = null;
                        type.BaseClass = null;
                        type.Module = null;
                        type.GenericArguments = new List<IntermediaryType>() { this.GenerateType(Type.GetElementType()) };
                    }
                    else if (this.IsDictionary(Type))
                    {
                        type.TypeName = "Dictionary";
                        type.NameSpace = null;
                        type.BaseClass = null;
                        type.Module = null;

                    }
                    else if (this.IsList(Type))
                    {
                        type.TypeName = "List";
                        type.NameSpace = null;
                        type.BaseClass = null;
                        type.Module = null;

                    }
                    else
                    {
                        type.TypeName = TypeName;
                    }

                    if (Type.IsGenericType)
                    {
                        type.TypeName = type.TypeName.Split('`').FirstOrDefault();
                        type.GenericArguments = Type.GetGenericArguments().Select(r => this.GenerateType(r)).ToList();
                    }

                    type.Properties = Type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).Select(r => this.GenerateProperty(r)).ToList();

                    if (type is IntermediaryAttribute attr)
                    {
                        ConstructorInfo constructorInfo = Type.GetConstructors().FirstOrDefault();

                        if (constructorInfo != null)
                        {
                            attr.Arguments = constructorInfo.GetParameters().Select(r => this.GenerateParameter(r)).ToList();
                        }
                    }

                }

                return type;
            }
            else
            {
                return this.TypeProcessing[Type];
            }
        }

        protected bool IsAttribute(Type Type)
        {
            return typeof(Attribute).IsAssignableFrom(Type);
        }

        protected bool IsList(Type Type)
        {
            return Type.GetInterface(nameof(IEnumerable)) != null && !this.IsArray(Type);
        }

        protected bool IsPrimitive(Type Type)
        {

            return Type.IsPrimitive;
        }


        protected bool IsArray(Type Type)
        {
            return Type.IsArray;
        }

        protected bool IsDictionary(Type Type)
        {
            return Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        protected string GetPrimitiveTypeName(Type Type)
        {
            string typeName = "object";

            if (Type == typeof(object))
            {
                typeName = "object";
            }
            else
            {
                typeName = Type.Name;
            }

            return typeName;
        }

        protected IntermediaryProperty GenerateProperty(PropertyInfo Property)
        {
            IntermediaryProperty prop = new IntermediaryProperty();
            prop.PropertyName = Property.Name;
            prop.Type = this.GenerateType(Property.PropertyType);
            return prop;
        }

        protected IntermediaryArgument GenerateParameter(ParameterInfo Parameter)
        {
            IntermediaryArgument arg = new IntermediaryArgument();
            arg.ArgumentName = Parameter.Name;
            arg.DefaultValue = Parameter.DefaultValue;
            arg.Type = this.GenerateType(Parameter.ParameterType);
            return arg;
        }
    }
}
