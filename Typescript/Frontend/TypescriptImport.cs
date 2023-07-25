using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.ModelGenerator.Typescript.Frontend
{
    public class TypescriptImport
    {
        public string TypeName { get; set; }

        public string ModuleName { get; set; }

        public string NameSpace { get; set; }

        public TypescriptImport(string TypeName, string ModuleName, string NameSpace) 
        {
            this.TypeName = TypeName;
            this.ModuleName = ModuleName;
            this.NameSpace = NameSpace;
        }
    }
}
