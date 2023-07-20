using System;
using System.Collections.Generic;
using System.Linq;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryType
    {
        public string TypeName { get; set; }

        public List<IntermediaryType> GenericArguments { get; set; }
        public IntermediaryType TypeConstraint { get; set; }

        public IntermediaryType BaseClass { get; set; }

        public List<IntermediaryType> Interfaces { get; set; } = new List<IntermediaryType>();

        public bool IsClass { get; set; }

        public bool IsInterface { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsGenericType { get; set; }

        public bool IsIntrinsic { get; set; }

        public bool IsPrimitive { get; set; }

        public List<IntermediaryProperty> Properties { get; set; } = new List<IntermediaryProperty>();

        public List<IntermediaryAttribute> Attributes { get; set; } = new List<IntermediaryAttribute>();

        public string Module { get; set; }

        public string NameSpace { get; set; }

        public override string ToString()
        {
            List<string> names = new List<string>();

            if(this.Module != null)
            {
                names.Add(this.Module);
            }
            if (this.NameSpace != null)
            {
                names.Add(this.NameSpace);
            }

            names.Add(this.TypeName);

            return String.Join(".", names.ToArray());
        }


    }
}
