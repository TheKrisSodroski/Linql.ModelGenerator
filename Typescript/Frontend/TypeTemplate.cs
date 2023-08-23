using Linql.ModelGenerator.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Linql.ModelGenerator.Typescript.Frontend
{
    public partial class LinqlModelGeneratorTypescriptFrontend
    {

        private static readonly Dictionary<Type, string> Aliases =
        new Dictionary<Type, string>()
        {
            { typeof(byte), "any" },
            { typeof(sbyte), "any" },
            { typeof(short), "number" },
            { typeof(ushort), "number" },
            { typeof(int), "number" },
            { typeof(uint), "number" },
            { typeof(long), "bigint" },
            { typeof(ulong), "bigint" },
            { typeof(float), "number" },
            { typeof(double), "number" },
            { typeof(decimal), "number" },
            { typeof(object), "any" },
            { typeof(bool), "boolean" },
            { typeof(char), "string" },
            { typeof(string), "string" },
            { typeof(void), "any" },
            { typeof(DateTime), "Date" }
        };

        private static readonly Dictionary<Type, string> DefaultValues =
new Dictionary<Type, string>()
{
            { typeof(byte), "0" },
            { typeof(sbyte), "0" },
            { typeof(short), "0" },
            { typeof(ushort), "0" },
            { typeof(int), "0" },
            { typeof(uint), "0" },
            { typeof(long), "0n" },
            { typeof(ulong), "0n" },
            { typeof(float), "0" },
            { typeof(double), "0" },
            { typeof(decimal), "0" },
            { typeof(bool), "false" },
            { typeof(char), "\"\"" },
            { typeof(string), "\"\"" },
            { typeof(void), "null" },
            { typeof(DateTime), "new Date()" }
};

        private static readonly List<string> ClassAttributes = new List<string>()
            {
                Enum.GetName(typeof(AttributeTargets), AttributeTargets.All),
                Enum.GetName(typeof(AttributeTargets), AttributeTargets.Class)
            };

        private static readonly List<string> PropertyAttributes = new List<string>()
            {
                Enum.GetName(typeof(AttributeTargets), AttributeTargets.All),
                Enum.GetName(typeof(AttributeTargets), AttributeTargets.Property)
            };
        private bool IsClassAttribute(CoreAttribute Attr)
        {
            return Attr.Targets.Any(s => LinqlModelGeneratorTypescriptFrontend.ClassAttributes.Contains(s));
        }

        private bool IsPropertyAttribute(CoreAttribute Attr)
        {
            return Attr.Targets.Any(s => LinqlModelGeneratorTypescriptFrontend.PropertyAttributes.Contains(s));
        }


        protected string GetAngularAppPath()
        {
            return Path.Combine(this.ProjectPath, this.Module.ModuleName);
        }

        protected string GetAngularLibRoot()
        {
            return Path.Combine(this.ProjectPath, this.Module.ModuleName, "projects", this.GetAngularLibraryName(this.Module.ModuleName));
        }

        protected string GetAngularSrcPath()
        {
            return Path.Combine(this.GetAngularLibRoot(), "src");
        }

        protected string GetAngularLibPath()
        {
            return Path.Combine(this.GetAngularSrcPath(), "lib");
        }

        protected string GetAngularLibraryName(string ModuleName)
        {
            string module = ModuleName;
            List<string> split = module.ToLower().Split('.').ToList();
            string endPart = String.Join("-", split.Skip(1));
            string first = split.First();
            return $"{first}.{endPart}";
        }

    }

    public static class DictionaryHelper
    {
        public static void Merge(this Dictionary<string, string> Input, Dictionary<string, string> DictionaryToMerge)
        {
            foreach (var key in DictionaryToMerge)
            {
                if (!Input.ContainsKey(key.Key))
                {
                    Input.Add(key.Key, key.Value);
                }
            }
        }
    }
}
