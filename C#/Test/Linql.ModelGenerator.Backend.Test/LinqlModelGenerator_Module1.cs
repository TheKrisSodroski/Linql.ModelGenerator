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
           
            Assert.That(this.Module.ModuleName, Is.EqualTo(typeof(PrimitiveClass).Assembly.GetName().Name));
            Assert.That(this.Module.BaseLanguage, Is.EqualTo("C#"));

        }

        [Test]
        public void Test_PrimitiveClass()
        {
            IntermediaryType basicType = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(PrimitiveClass));

            Assert.That(basicType, Is.Not.EqualTo(null));
            Assert.That(basicType.IsClass, Is.True);
            Assert.That(basicType.IsAbstract, Is.False);
            Assert.That(basicType.IsInterface, Is.False);

            basicType.Properties.ForEach(r =>
            {
                Assert.That(r.Type.TypeName, Is.EqualTo(r.PropertyName));
            });
        }

        [Test]
        public void Test_PrimitiveInterface()
        {
            IntermediaryType basicInterface = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(IPrimitiveInterface));

            Assert.That(basicInterface, Is.Not.EqualTo(null));
            Assert.That(basicInterface.IsClass, Is.False);
            Assert.That(basicInterface.IsAbstract, Is.True);
            Assert.That(basicInterface.IsInterface, Is.True);

            basicInterface.Properties.ForEach(r =>
            {
                Assert.That(r.Type.TypeName, Is.EqualTo(r.PropertyName));
            });
        }

        [Test]
        public void Test_PrimitiveAbstract()
        {
            IntermediaryType basicInterface = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(PrimitiveAbstract));

            Assert.That(basicInterface, Is.Not.EqualTo(null));
            Assert.That(basicInterface.IsClass, Is.True);
            Assert.That(basicInterface.IsAbstract, Is.True);
            Assert.That(basicInterface.IsInterface, Is.False);

            basicInterface.Properties.ForEach(r =>
            {
                Assert.That(r.Type.TypeName, Is.EqualTo(r.PropertyName));
            });
        }



    }
}