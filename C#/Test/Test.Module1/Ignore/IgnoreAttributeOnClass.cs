using Linql.ModelGenerator.CSharp.Backend;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Module1.Ignore
{
    [IgnoreAttribute]
    public class IgnoreAttributeOnClass
    {
        public string PropertyOne { get; set; }
    }
}
