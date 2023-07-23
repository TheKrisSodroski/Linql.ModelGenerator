﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.Frontend
{
    public partial class LinqlFrontendModelGenerator
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
}
