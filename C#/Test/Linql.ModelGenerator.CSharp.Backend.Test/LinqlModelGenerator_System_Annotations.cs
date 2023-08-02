using Linql.ModelGenerator.Core;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1.Inheritance;
using Test.Module1;
using Test.Module1.Generics;
using Test.Module2;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace Linql.ModelGenerator.CSharp.Backend.Test
{
    public class LinqlModelGenerator_SystemAnnotations : BaseModelGeneratorTest
    {
        public override void SetUp()
        {
        }


        [Test]
        public void ShouldGenerate()
        {
            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(typeof(KeyAttribute).Assembly);
            this.Module = generator.Generate();
        }

    }
}