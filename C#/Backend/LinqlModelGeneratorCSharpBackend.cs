using Linql.ModelGenerator.Core;
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

        protected Dictionary<Type, CoreType> TypeProcessing { get; set; } = new Dictionary<Type, CoreType>();

        public List<IModuleOverridePlugin> OverridePlugins { get; set; } = new List<IModuleOverridePlugin>();

        public List<IPrimitiveTypePlugin> PrimitiveTypePlugins { get; set; } = new List<IPrimitiveTypePlugin>();

        public LinqlModelGeneratorCSharpBackend(
            Assembly Assembly
            )
        {
            this.Assembly = Assembly;
            this.OverridePlugins.Add(new DefaultOverridePlugin());
            this.PrimitiveTypePlugins.Add(new DefaultPrimitiveTypePlugin());
        }

        public LinqlModelGeneratorCSharpBackend(string AssemblyPath)
        {
            string assemblyPath = AssemblyPath;
            if (!AssemblyPath.EndsWith(".dll"))
            {
                List<string> files = Directory.GetFiles(AssemblyPath, "*.csproj", SearchOption.AllDirectories).ToList();

                Console.WriteLine("Found the following csproj files:");

                files.ForEach(r =>
                {
                    Console.WriteLine(r);
                });

                string csProj = files.FirstOrDefault());

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

                    List<string> foundDlls = Directory.GetFiles(AssemblyPath, $"{moduleName}.dll", SearchOption.AllDirectories).ToList();
                    foundDlls = foundDlls.Where(r => r.Contains("publish")).ToList();
                    assemblyPath = foundDlls.FirstOrDefault(r => r.Contains("Release"));

                    if(assemblyPath == null)
                    {
                        assemblyPath = foundDlls.FirstOrDefault();
                    }
                }
                else
                {
                    throw new FileNotFoundException($"Unable to find csproj file in directory {AssemblyPath}");
                }

            }

            string fullPath = Path.GetFullPath(assemblyPath);
            this.Assembly = Assembly.LoadFrom(fullPath);
            this.OverridePlugins.Add(new DefaultOverridePlugin());
            this.PrimitiveTypePlugins.Add(new DefaultPrimitiveTypePlugin());

        }

        protected virtual bool IsValidType(Type Type)
        {
            return this.OverridePlugins.All(s => s.IsValidType(Type));
        }

        protected virtual bool IsObjectType(Type Type)
        {
            return this.OverridePlugins.Any(s => s.IsObjectType(Type));
        }

        protected virtual bool IsValidProperty(Type Type, PropertyInfo Property)
        {
            return this.OverridePlugins.All(s => s.IsValidProperty(Type, Property));
        }


        public virtual CoreModule Generate()
        {
            CoreModule module = new CoreModule();
            module.BaseLanguage = "C#";
            AssemblyName assemName = this.Assembly.GetName();
            module.ModuleName = assemName.Name;
            module.Version = this.GetAssemblyVersion(this.Assembly);

            module.Types = this.GenerateTypes();
            return module;
        }

        protected virtual List<CoreType> GenerateTypes()
        {
            List<Type> typesToGenerate = this.Assembly.GetTypes().ToList();
            return typesToGenerate
              .Where(r => this.IsValidType(r))
              .Select(r => this.GenerateType(r)).ToList();
        }

        protected string GetAssemblyVersion(Assembly Assembly)
        {
            List<IModuleOverridePlugin> overrides = this.OverridePlugins.ToList();
            overrides.Reverse();

            return overrides.Select(r => r.ModuleVersionOverride(Assembly)).FirstOrDefault(r => r != null);

        }

        protected CoreType GenerateType(Type Type)
        {
            string TypeName = Type.Name;
            if (!this.TypeProcessing.ContainsKey(Type))
            {
                CoreType type;

                if (this.IsAttribute(Type))
                {
                    type = new CoreAttribute();
                }
                else if (this.IsEnum(Type))
                {
                    type = new CoreEnum();
                }
                else
                {
                    type = new CoreType();
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
                    type.GenericArguments = new List<CoreType>() { this.GenerateType(Type.GetElementType()) };
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
                else if(this.IsObjectType(Type))
                {
                    type.TypeName = "object";
                    type.IsIntrinsic = true;
                    type.Module = null;
                    type.NameSpace = null;
                }
                else if (Type.IsGenericParameter == true)
                {
                    type.Module = null;
                    type.TypeName = TypeName;
                }
                else if (!this.IsValidType(Type))
                {
                    type = this.GenerateType(typeof(object));
                    return type;
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

                if (type.IsPrimitive == false)
                {
                    type.NameSpace = Type.Namespace;
                }

                if (this.TypeIsInModule(Type))
                {
                    if (Type.BaseType != null && this.IsValidType(Type.BaseType))
                    {
                        type.BaseClass = this.GenerateReducedType(Type.BaseType);
                    }

                    List<Type> interfaces = Type.GetInterfaces().Where(r => this.IsValidType(r)).ToList();
                    List<Type> baseTypeInterfaces = new List<Type>();
                    if (Type.BaseType != null)
                    {
                        baseTypeInterfaces.AddRange(Type.BaseType.GetInterfaces().Where(r => this.IsValidType(r)));
                    }

                    interfaces = interfaces
                        .Except(baseTypeInterfaces)
                        .Except(interfaces.SelectMany(s => s.GetInterfaces())).ToList();

                    type.Interfaces = interfaces.Select(r => this.GenerateReducedType(r)).ToList();

                    type.Properties = Type
                        .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                        .Where(r => this.IsValidProperty(Type, r))
                        .Select(r => this.GenerateProperty(r)).ToList();

                    if (type is CoreAttribute attr)
                    {
                        ConstructorInfo constructorInfo = Type.GetConstructors().FirstOrDefault();
                        List<PropertyInfo> properties = Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ToList();
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
                            List<string> names = Enum.GetNames(typeof(AttributeTargets)).ToList();
                            attrUsage.ForEach(r =>
                            {
                                foreach (string name in names)
                                {
                                    AttributeTargets target = (AttributeTargets)Enum.Parse(typeof(AttributeTargets), name);

                                    if (attr.Targets == null)
                                    {
                                        attr.Targets = new List<string>();
                                    }

                                    if (((int)r.ValidOn & (int)target) == 1)
                                    {
                                        attr.Targets.Add(Enum.GetName(typeof(AttributeTargets), target));
                                    }
                                    if (target == AttributeTargets.All)
                                    {
                                        attr.Targets = new List<string>() { Enum.GetName(typeof(AttributeTargets), AttributeTargets.All) };
                                        break;
                                    }

                                }

                            });
                        }

                        List<ParameterInfo> parameters = new List<ParameterInfo>();

                        if (constructorInfo != null)
                        {
                            parameters = constructorInfo.GetParameters().ToList();
                            attr.RequiredArguments = parameters.Where(r => !this.IsObjectType(r.ParameterType)).Select(r => this.GenerateParameter(r)).ToList();        
                        }

                        List<PropertyInfo> optionalArguments = properties.Where(r => !parameters.Select(s => s.Name.ToLower()).Contains(r.Name.ToLower())).ToList();

                        attr.OptionalArguments = optionalArguments.Select(r => this.GenerateParameterFromProperty(r)).ToList();

                        if (attr.RequiredArguments?.Count() == 0)
                        {
                            attr.RequiredArguments = null;
                        }
                        if(attr.OptionalArguments?.Count() == 0)
                        {
                            attr.OptionalArguments = null;
                        }
                    }
                    else if (type is CoreEnum enumType)
                    {
                        List<int> enumValues = Enum.GetValues(Type).Cast<int>().ToList();
                        enumType.Values = Enum.GetNames(Type).ToDictionary(r => r, r => (object)(int)Enum.Parse(Type, r));
                    }
                    else
                    {
                        type.Attributes = Type.GetCustomAttributes().Where(r => this.IsValidType(r.GetType())).Select(r => this.GenerateAttributeInstance(r)).ToList();
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

        protected bool IsEnum(Type Type)
        {
            return Type.IsEnum;
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

        protected CoreProperty GenerateProperty(PropertyInfo Property)
        {
            CoreProperty prop = new CoreProperty();
            prop.PropertyName = Property.Name;
            prop.Type = this.GenerateReducedType(Property.PropertyType);
            prop.Attributes = Property.GetCustomAttributes().Where(r => this.IsValidType(r.GetType())).Select(r => this.GenerateAttributeInstance(r)).ToList();
            prop.Overriden = Property.GetGetMethod().GetBaseDefinition().DeclaringType != Property.DeclaringType;
            prop.Virtual = Property.GetGetMethod().IsVirtual;

            if (prop.Attributes.Count() == 0)
            {
                prop.Attributes = null;
            }

            return prop;
        }

        protected CoreType GenerateReducedType(Type Type)
        {
            CoreType fullType = this.GenerateType(Type);
            return this.GenerateReducedType(fullType);
        }

        protected CoreType GenerateReducedType(CoreType FullType)
        {
            CoreType reducedType = new CoreType();
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

        protected CoreAttributeInstance GenerateAttributeInstance(Attribute Attribute)
        {
            Type attrType = Attribute.GetType();
            CoreAttributeInstance attr = new CoreAttributeInstance();
            CoreType type = this.GenerateType(attrType);

            attr.TypeName = type.TypeName;
            attr.NameSpace = type.NameSpace;
            attr.Module = type.Module;
            attr.ModuleVersion = this.GetAssemblyVersion(Attribute.GetType().Assembly);

            ConstructorInfo constructorInfo = Attribute.GetType().GetConstructors().FirstOrDefault();
            List<PropertyInfo> properties = attrType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).ToList();
            List<string> argNames = properties.Select(r => r.Name.ToLower()).ToList();
            if (constructorInfo != null)
            {
                argNames = constructorInfo.GetParameters().Where(r => r.Name != null).Select(r => r.Name.ToLower()).ToList();
            }
            //var test = properties.Where(r => argNames.Contains(r.Name.ToLower())).ToList();
            //var test2 = test.Where(r => !this.IsObjectType(r.PropertyType)).ToList();

            //attr.Arguments = attrType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
            //    .Where(r => argNames.Contains(r.Name.ToLower()))
            //    .Where(r => !this.IsObjectType(r.PropertyType))
            //    .ToDictionary(r => r.Name, r => r.GetValue(Attribute));

            attr.Arguments = properties
                .Where(r => !this.IsObjectType(r.PropertyType))
                .Where(r => this.GetAttributeValue(Attribute, r) != null)
                            .ToDictionary(r => r.Name, r => this.GetAttributeValue(Attribute, r));

            return attr;
        }

        protected object GetAttributeValue(Attribute Attribute, PropertyInfo Property)
        {
            try
            {
                return Property.GetValue(Attribute);
            }
            catch
            {
                return null;
            }
        }

        protected CoreArgument GenerateParameter(ParameterInfo Parameter)
        {
            CoreArgument arg = new CoreArgument();
            arg.ArgumentName = Parameter.Name;

            if (Parameter.DefaultValue != null && Parameter.DefaultValue.GetType() != typeof(DBNull))
            {
                arg.DefaultValue = Parameter.DefaultValue;
            }
            arg.Type = this.GenerateType(Parameter.ParameterType);
            return arg;
        }

        protected CoreArgument GenerateParameterFromProperty(PropertyInfo Parameter)
        {
            CoreArgument arg = new CoreArgument();
            arg.ArgumentName = Parameter.Name;
            arg.Type = this.GenerateType(Parameter.PropertyType);
            return arg;
        }
    }
}
