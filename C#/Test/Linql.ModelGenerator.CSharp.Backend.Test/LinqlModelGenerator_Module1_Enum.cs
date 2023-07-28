using Linql.ModelGenerator.Core;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1;
using Test.Module1.Enums;
using Test.Module1.Inheritance;

namespace Linql.ModelGenerator.CSharp.Backend.Test
{
    public class LinqlModelGenerator_Module1_Enum : BaseModelGeneratorTest
    {

        [Test]
        public void Enum()
        {
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(IntEnum));
            
            Assert.That(type is CoreEnum, Is.True);
            
            CoreEnum Enum = (CoreEnum)type;
            
            Assert.That(Enum.Values.Count, Is.GreaterThan(0));
        }


    }
}