using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Module1
{
    public interface IPrimitiveInterface
    {
        int Int32 { get; set; }

        string String { get; set; }

        decimal Decimal { get; set; }

        float Single { get; set; }

        bool Boolean { get; set; }

        char Char { get; set; }

        double Double { get; set; }

        long Int64 { get; set; }

        byte Byte { get; set; }

        short Int16 { get; set; } 

        object Object { get; set; }
    }
}
