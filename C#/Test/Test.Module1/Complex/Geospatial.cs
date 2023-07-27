using System;
using System.Collections.Generic;
using System.Spatial;
using System.Text;
using Test.Module1.Generics;
using Test.Module1.Inheritance;

namespace Test.Module1.Complex
{
    public class Geospatial
    {
        public Geography Geography { get; set; }

        public Geometry Geometry { get; set; }

        public GenericOne<Geography> NestedGeography { get; set; }

    }
}
