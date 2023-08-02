using Linql.ModelGenerator.Core;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1.Inheritance;
using Test.Module1;
using Test.Module1.Generics;
using Test.Module2;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using Linql.ComponentModel.DataAnnotations;

namespace Linql.ModelGenerator.CSharp.Backend.Test
{
    public class LinqlModelGenerator_Annotations : BaseModelGeneratorTest
    {
        protected override string ModuleName { get; set; } = "Test.Annotations";
        public override void SetUp()
        {
            LinqlModelGeneratorCSharpBackend annotationsGen = new LinqlModelGeneratorCSharpBackend(typeof(KeyAttribute).Assembly);
            annotationsGen.ValidTypePlugins.Add(new LinqlDataAnnotationsIgnore());
            CoreModule annotations = annotationsGen.Generate();

            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(Path.Combine(this.ModelsPath, this.ModuleName));
            generator.ValidTypePlugins.Add(new LinqlDataAnnotationsIgnore());
            this.Module = generator.Generate();

        }


        [Test]
        public void ShouldGenerate()
        {
            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(typeof(KeyAttribute).Assembly);
            this.Module = generator.Generate();
        }

    }
}