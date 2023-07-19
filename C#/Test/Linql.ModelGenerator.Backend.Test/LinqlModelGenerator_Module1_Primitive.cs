using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1;

namespace Linql.ModelGenerator.Backend.Test
{
    public class LinqlModelGenerator_Module1_Primitive : BaseModelGeneratorTest
    {

        [Test]
        public void Generate()
        {
           
            Assert.That(this.Module.ModuleName, Is.EqualTo(typeof(PrimitiveClass).Assembly.GetName().Name));
            Assert.That(this.Module.BaseLanguage, Is.EqualTo("C#"));

        }

        [Test]
        public void PrimitiveClassTest()
        {
            IntermediaryType basicType = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(PrimitiveClass));

            Assert.That(basicType, Is.Not.EqualTo(null));
            Assert.That(basicType.IsClass, Is.True);
            Assert.That(basicType.IsAbstract, Is.False);
            Assert.That(basicType.IsInterface, Is.False);
            Assert.That(basicType.ModulePath, Is.EqualTo(typeof(PrimitiveClass).Namespace));

            basicType.Properties.ForEach(r =>
            {
                if (r.PropertyName != nameof(PrimitiveClass.Object))
                {
                    Assert.That(r.Type.TypeName, Is.EqualTo(r.PropertyName));
                }
                Assert.That(r.Type.ModulePath, Is.EqualTo(null));

            });
        }

        [Test]
        public void PrimitiveInterfaceTest()
        {
            IntermediaryType basicInterface = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(IPrimitiveInterface));

            Assert.That(basicInterface, Is.Not.EqualTo(null));
            Assert.That(basicInterface.IsClass, Is.False);
            Assert.That(basicInterface.IsAbstract, Is.True);
            Assert.That(basicInterface.IsInterface, Is.True);
            Assert.That(basicInterface.ModulePath, Is.EqualTo(typeof(IPrimitiveInterface).Namespace));

            basicInterface.Properties.ForEach(r =>
            {
                if (r.PropertyName != nameof(IPrimitiveInterface.Object))
                {
                    Assert.That(r.Type.TypeName, Is.EqualTo(r.PropertyName));
                }
                Assert.That(r.Type.ModulePath, Is.EqualTo(null));
            });
        }

        [Test]
        public void PrimitiveAbstractTest()
        {
            IntermediaryType basicInterface = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(PrimitiveAbstract));

            Assert.That(basicInterface, Is.Not.EqualTo(null));
            Assert.That(basicInterface.IsClass, Is.True);
            Assert.That(basicInterface.IsAbstract, Is.True);
            Assert.That(basicInterface.IsInterface, Is.False);
            Assert.That(basicInterface.ModulePath, Is.EqualTo(typeof(PrimitiveAbstract).Namespace));

            basicInterface.Properties.ForEach(r =>
            {
                if (r.PropertyName != nameof(PrimitiveAbstract.Object))
                {
                    Assert.That(r.Type.TypeName, Is.EqualTo(r.PropertyName));
                }
                Assert.That(r.Type.ModulePath, Is.EqualTo(null));
            });
        }

    }
}