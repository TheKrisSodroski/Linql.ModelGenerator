using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1;

namespace Linql.ModelGenerator.Backend.Test
{
    public class LinqlModelGenerator_Module1 : BaseModelGeneratorTest
    {

        protected IntermediaryModule Module { get; set; }

        [OneTimeSetUp] public void SetUp()
        {
            LinqlModelGenerator generator = new LinqlModelGenerator(Path.Combine(this.ModelsPath, "Test.Module1"));
            this.Module = generator.Generate();
        }

        [Test]
        public void Generate()
        {
           
            Assert.That(this.Module.ModuleName == typeof(BasicClass).Assembly.GetName().Name);
            Assert.That(this.Module.BaseLanguage == "C#");

            IntermediaryType basicType = Module.Types.FirstOrDefault(r => r.TypeName == nameof(BasicClass));

            Assert.That(basicType != null);
            Assert.That(basicType.IsClass == true && basicType.IsAbstract == false && basicType.IsInterface == false);
        }

        [Test]
        public void BasicClass()
        {

            IntermediaryType basicType = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(BasicClass));

            Assert.That(basicType != null);
            Assert.That(basicType.IsClass == true && basicType.IsAbstract == false && basicType.IsInterface == false);
        }

    }
}