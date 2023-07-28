using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.CSharp.Frontend
{
    public partial class LinqlModelGeneratorCSharpFrontend
    {
        protected List<string> Usings { get; set; } = new List<string>()
        {
            "using System;",
            "using System.Collections.Generic;",
            "using System.Text;"
        };

        private static readonly Dictionary<Type, string> Aliases =
        new Dictionary<Type, string>()
        {
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(object), "object" },
            { typeof(bool), "bool" },
            { typeof(char), "char" },
            { typeof(string), "string" },
            { typeof(void), "void" }
        };
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
