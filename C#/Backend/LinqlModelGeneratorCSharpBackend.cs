using Linql.ModelGenerator.Intermediary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Linql.ModelGenerator.CSharp.Backend
{
    public class LinqlModelGeneratorCSharpBackend
    {
        public Assembly Assembly { get; set; }

        protected Dictionary<Type, IntermediaryType> TypeProcessing { get; set; } = new Dictionary<Type, IntermediaryType>();

        protected List<IIgnoreTypePlugin> IgnoreTypePlugins { get; set; } = new List<IIgnoreTypePlugin>();

        protected List<IPrimitiveTypePlugin> PrimitiveTypePlugins { get; set; } = new List<IPrimitiveTypePlugin>();

        public LinqlModelGeneratorCSharpBackend(
            Assembly Assembly
            )
        {
            this.Assembly = Assembly;
            this.IgnoreTypePlugins.Add(new DefaultIgnoreTypePlugin());
            this.PrimitiveTypePlugins.Add(new DefaultPrimitiveTypePlugin());
        }

        public LinqlModelGeneratorCSharpBackend(string AssemblyPath)
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
                    process.WaitForExit();

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
            this.Assembly = Assembly.LoadFrom(fullPath);
            this.IgnoreTypePlugins.Add(new DefaultIgnoreTypePlugin());
            this.PrimitiveTypePlugins.Add(new DefaultPrimitiveTypePlugin());

        }

        public IntermediaryModule Generate()
        {
            IntermediaryModule module = new IntermediaryModule();
            module.BaseLanguage = "C#";
            AssemblyName assemName = this.Assembly.GetName();
            module.ModuleName = assemName.Name;
            module.Version = this.GetAssemblyVersion(this.Assembly);

            module.Types = this.GenerateTypes();
            return module;
        }

        protected List<IntermediaryType> GenerateTypes()
        {
            List<Type> typesToGenerate = this.Assembly.GetTypes().ToList();
            return typesToGenerate
              .Where(r => !this.IgnoreTypePlugins.Any(s => s.IgnoreType(r)))
              .Select(r => this.GenerateType(r)).ToList();
        }

        protected string GetAssemblyVersion(Assembly Assembly)
        {
            AssemblyInformationalVersionAttribute version = Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return version.InformationalVersion;
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

                type.Module = Type.Assembly.GetName().Name;


                if (this.IsPrimitive(Type) || this.PrimitiveTypePlugins.Any(s => s.IsPrimitiveType(Type)))
                {
                    type.IsPrimitive = true;
                    type.TypeName = this.GetPrimitiveTypeName(Type);
                    type.Module = null;
                }
                else if (this.IsArray(Type))
                {
                    type.TypeName = "Array";
                    type.IsIntrinsic = true;
                    type.Module = null;
                    type.GenericArguments = new List<IntermediaryType>() { this.GenerateType(Type.GetElementType()) };
                }
                else if (this.IsDictionary(Type))
                {
                    type.TypeName = "Dictionary";
                    type.IsIntrinsic = true;
                    type.Module = null;

                }
                else if (this.IsList(Type))
                {
                    type.TypeName = "List";
                    type.IsIntrinsic = true;
                    type.Module = null;
                }
                else if (Type.IsGenericParameter == true)
                {
                    type.Module = null;
                    type.TypeName = TypeName;
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

                type.IsGenericType = Type.IsGenericType;
                type.IsClass = Type.IsClass;
                type.IsInterface = Type.IsInterface;
                type.IsAbstract = Type.IsAbstract;

                if (this.TypeIsInModule(Type))
                {
                    type.NameSpace = Type.Namespace;

                    if (Type.BaseType != null && !this.IgnoreTypePlugins.Any(s => s.IgnoreType(Type.BaseType)))
                    {
                        type.BaseClass = this.GenerateReducedType(Type.BaseType);
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

                    type.Interfaces = interfaces.Select(r => this.GenerateReducedType(r)).ToList();

                    type.Properties = Type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).Select(r => this.GenerateProperty(r)).ToList();

                    if (type is IntermediaryAttribute attr)
                    {
                        ConstructorInfo constructorInfo = Type.GetConstructors().FirstOrDefault();

                        List<AttributeUsageAttribute> attrUsage = Type.GetCustomAttributes(typeof(AttributeUsageAttribute)).Cast<AttributeUsageAttribute>().ToList();

                        if (attrUsage.Count == 0)
                        {
                            attr.Targets = new List<string>()
                            {
                                Enum.GetName(typeof(AttributeTargets), AttributeTargets.Class),
                                Enum.GetName(typeof(AttributeTargets), AttributeTargets.Property),                            
                            };
                        }
                        else
                        {
                            attr.Targets = attrUsage.Select(r => Enum.GetName(typeof(AttributeTargets), r.ValidOn)).ToList();
                        }

                        if (constructorInfo != null)
                        {
                            attr.Arguments = constructorInfo.GetParameters().Select(r => this.GenerateParameter(r)).ToList();

                            if (attr.Arguments?.Count() == 0)
                            {
                                attr.Arguments = null;
                            }
                        }
                    }
                    else
                    {
                        type.Attributes = Type.GetCustomAttributes().Select(r => this.GenerateAttributeInstance(r)).ToList();
                    }
                }
                else if (type.IsPrimitive == false && type.IsIntrinsic == false)
                {
                    type.ModuleVersion = this.GetAssemblyVersion(Type.Assembly);
                }

                if (type.Properties?.Count() == 0)
                {
                    type.Properties = null;
                }
                if (type.Interfaces?.Count() == 0)
                {
                    type.Interfaces = null;
                }
                if (type.Attributes?.Count() == 0)
                {
                    type.Attributes = null;
                }


                return type;
            }
            else
            {
                return this.TypeProcessing[Type];
            }
        }

        protected bool TypeIsInModule(Type Type)
        {
            return Type.Assembly == this.Assembly;
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
            prop.Type = this.GenerateReducedType(Property.PropertyType);
            prop.Attributes = Property.GetCustomAttributes().Select(r => this.GenerateAttributeInstance(r)).ToList();
            prop.Overriden = Property.GetGetMethod().GetBaseDefinition().DeclaringType != Property.DeclaringType;
            prop.Virtual = Property.GetGetMethod().IsVirtual;

            if (prop.Attributes.Count() == 0)
            {
                prop.Attributes = null;
            }

            return prop;
        }

        protected IntermediaryType GenerateReducedType(Type Type)
        {
            IntermediaryType fullType = this.GenerateType(Type);
            return this.GenerateReducedType(fullType);
        }

        protected IntermediaryType GenerateReducedType(IntermediaryType FullType)
        {
            IntermediaryType reducedType = new IntermediaryType();
            reducedType.Module = FullType.Module;
            reducedType.ModuleVersion = FullType.ModuleVersion;

            if (FullType.GenericArguments != null)
            {
                reducedType.GenericArguments = FullType.GenericArguments.Select(r => this.GenerateReducedType(r)).ToList();
            }
            reducedType.IsPrimitive = FullType.IsPrimitive;
            reducedType.IsIntrinsic = FullType.IsIntrinsic;
            reducedType.NameSpace = FullType.NameSpace;
            reducedType.TypeName = FullType.TypeName;
            return reducedType;
        }

        protected IntermediaryAttributeInstance GenerateAttributeInstance(Attribute Attribute)
        {
            Type attrType = Attribute.GetType();
            IntermediaryAttributeInstance attr = new IntermediaryAttributeInstance();
            IntermediaryType type = this.GenerateType(attrType);

            attr.TypeName = type.TypeName;
            attr.NameSpace = type.NameSpace;
            attr.Module = type.Module;
            attr.Arguments = attrType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).ToDictionary(r => r.Name, r => r.GetValue(Attribute));

            return attr;
        }

        protected IntermediaryArgument GenerateParameter(ParameterInfo Parameter)
        {
            IntermediaryArgument arg = new IntermediaryArgument();
            arg.ArgumentName = Parameter.Name;

            if (Parameter.DefaultValue != null && Parameter.DefaultValue.GetType() != typeof(DBNull))
            {
                arg.DefaultValue = Parameter.DefaultValue;
            }
            arg.Type = this.GenerateType(Parameter.ParameterType);
            return arg;
        }
    }
}
