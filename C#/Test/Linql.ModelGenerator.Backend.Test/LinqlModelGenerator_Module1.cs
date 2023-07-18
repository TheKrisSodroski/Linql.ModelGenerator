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
           
            Assert.That(this.Module.ModuleName == typeof(PrimitiveClass).Assembly.GetName().Name);
            Assert.That(this.Module.BaseLanguage == "C#");

        }

        [Test]
        public void Test_PrimitiveClass()
        {

            IntermediaryType basicType = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(PrimitiveClass));

            Assert.That(basicType != null);
            Assert.That(basicType.IsClass == true && basicType.IsAbstract == false && basicType.IsInterface == false);

            IntermediaryProperty prop = basicType.Properties.FirstOrDefault(r => r.PropertyName == nameof(PrimitiveClass.Int));
            Assert.That(prop != null && prop.Type.TypeName == "int");

        }

    }
}