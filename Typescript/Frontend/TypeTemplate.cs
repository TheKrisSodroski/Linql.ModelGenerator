﻿using System;
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
            { typeof(void), "any" }
        };

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
            foreach(var key in DictionaryToMerge)
            {
                if (!Input.ContainsKey(key.Key))
                {
                    Input.Add(key.Key, key.Value);
                }
            }
        }
    }
}