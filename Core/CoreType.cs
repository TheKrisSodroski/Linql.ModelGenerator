using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Linql.ModelGenerator.Core
{
    [JsonDerivedType(typeof(CoreAttribute), typeDiscriminator: nameof(CoreAttribute))]
    [JsonDerivedType(typeof(CoreEnum), typeDiscriminator: nameof(CoreEnum))]
    public class CoreType
    {
        public string TypeName { get; set; }

        public List<CoreType> GenericArguments { get; set; }
        public CoreType TypeConstraint { get; set; }

        public CoreType BaseClass { get; set; }

        public List<CoreType> Interfaces { get; set; }

        public bool IsClass { get; set; }

        public bool IsInterface { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsGenericType { get; set; }

        public bool IsIntrinsic { get; set; }

        public bool IsPrimitive { get; set; }

        public List<CoreProperty> Properties { get; set; }

        public List<CoreAttributeInstance> Attributes { get; set; }

        public string Module { get; set; }

        public string ModuleVersion { get; set; }

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
