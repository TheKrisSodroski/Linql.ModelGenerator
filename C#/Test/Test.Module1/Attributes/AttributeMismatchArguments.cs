using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Module1.Attributes
{
    public class AttributeMismatchArguments : Attribute
    {
        public string ArgumentOne { get; set; }

        public bool ArgumentTwo { get; set; }

        public AttributeMismatchArguments(string ArgumentOne, bool ArgumentTwo, string ArgumentThree) 
        {
            this.ArgumentOne = ArgumentOne;
            this.ArgumentTwo = ArgumentTwo;
        }
    }
}
