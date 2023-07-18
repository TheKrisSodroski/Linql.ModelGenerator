using Linql.ModelGenerator.Intermediary;
using System;
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

        public LinqlModelGenerator(Assembly Assembly)
        {
            this.Assembly = Assembly;
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
        }

        public IntermediaryModule Generate()
        {
            IntermediaryModule module = new IntermediaryModule();
            module.BaseLanguage = "C#";
            module.ModuleName = Assembly.GetName().Name;
            module.Types = Assembly.GetTypes().Select(r => this.GenerateType(r)).ToList();
            return module;
        }

        protected IntermediaryType GenerateType(Type Type)
        {
            string TypeName = Type.Name;
            if (!this.TypeProcessing.ContainsKey(Type))
            {
                IntermediaryType type = new IntermediaryType();
                type.InternalPath = Type.Namespace;

                if (Type.BaseType != null)
                {
                    type.BaseClass = this.GenerateType(Type.BaseType);
                }

                List<Type> interfaces = Type.GetInterfaces().Where(r => r.Assembly != typeof(IComparable).Assembly).ToList();
                interfaces = interfaces.Except(interfaces.SelectMany(s => s.GetInterfaces())).ToList(); 
                type.Interfaces = interfaces.Select(r => this.GenerateType(r)).ToList();

                if (Type.IsPrimitive)
                {
                    type.IsPrimitive = true;
                    type.TypeName = this.GetPrimitiveTypeName(Type);
                }
                else
                {
                    type.TypeName = TypeName;
                    this.TypeProcessing.Add(Type, type);
                    type.IsClass = Type.IsClass;
                    type.IsInterface = Type.IsInterface;
                    type.IsAbstract = Type.IsAbstract;
                    type.Properties = Type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).Select(r => this.GenerateProperty(r)).ToList();

                }

                return type;
            }
            else
            {
                return this.TypeProcessing[Type];
            }
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
    }
}
