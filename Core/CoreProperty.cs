using System;
using System.Collections.Generic;
using System.Reflection;

namespace Linql.ModelGenerator.Core
{
    public class CoreProperty
    {
        public string PropertyName { get; set; }

        public List<CoreAttributeInstance> Attributes { get; set; }

        public CoreType Type { get; set; }

        public bool Overriden { get; set; }

        public bool Virtual { get; set; }

        public override string ToString()
        {
            return $"{this.PropertyName} - {this.Type.ToString()}";
        }

    }
}
