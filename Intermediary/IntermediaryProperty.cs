﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Linql.ModelGenerator.Intermediary
{
    public class IntermediaryProperty
    {
        public string PropertyName { get; set; }

        public List<IntermediaryAttributeInstance> Attributes { get; set; }

        public IntermediaryType Type { get; set; }

        public bool Overriden { get; set; }

        public bool Virtual { get; set; }

        public override string ToString()
        {
            return $"{this.PropertyName} - {this.Type.ToString()}";
        }

    }
}