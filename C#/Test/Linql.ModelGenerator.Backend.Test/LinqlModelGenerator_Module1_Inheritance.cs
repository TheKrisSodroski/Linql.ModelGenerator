using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1;
using Test.Module1.Inheritance;

namespace Linql.ModelGenerator.Backend.Test
{
    public class LinqlModelGenerator_Module1_Inheritance : BaseModelGeneratorTest
    {

        [Test]
        public void Test_InheritedAbstract()
        {
            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(InheritAbstract));
            IntermediaryType baseType = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(PrimitiveAbstract));

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.True);
            Assert.That(type.IsInterface, Is.False);

            Assert.That(type.Properties.Count(), Is.EqualTo(0));

            Assert.That(type.BaseClass, Is.EqualTo(baseType));


        }

        [Test]
        public void Test_DoubleInheritedAbstract()
        {
            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(DoubleInheritAbstract));
            IntermediaryType baseType = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(InheritAbstract));

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.True);
            Assert.That(type.IsInterface, Is.False);

            Assert.That(type.Properties.Count(), Is.EqualTo(1));

            Assert.That(type.BaseClass, Is.EqualTo(baseType));

        }

        [Test]
        public void Test_TrippleInheritWithInterface()
        {
            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(TripleInheritWithInterface));
            IntermediaryType baseType = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(DoubleInheritAbstract));
            IntermediaryType primitiveInterface = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(IPrimitiveInterface));

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.True);
            Assert.That(type.IsInterface, Is.False);

            Assert.That(type.Properties.Count(), Is.EqualTo(1));
            Assert.That(type.Interfaces.Count(), Is.EqualTo(1));

            Assert.That(type.Interfaces.Contains(primitiveInterface), Is.True);


            Assert.That(type.BaseClass, Is.EqualTo(baseType));

        }


    }
}