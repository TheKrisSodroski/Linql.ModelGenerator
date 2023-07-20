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
        public void InheritedAbstract()
        {
            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(InheritAbstract));
            IntermediaryType baseType = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(PrimitiveAbstract));

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.True);
            Assert.That(type.IsInterface, Is.False);

            Assert.That(type.Properties, Is.EqualTo(null));

            Assert.That(type.BaseClass.TypeName, Is.EqualTo(baseType.TypeName));


        }

        [Test]
        public void DoubleInheritedAbstract()
        {
            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(DoubleInheritAbstract));
            IntermediaryType baseType = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(InheritAbstract));

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.True);
            Assert.That(type.IsInterface, Is.False);

            Assert.That(type.Properties.Count(), Is.EqualTo(1));

            Assert.That(type.BaseClass.TypeName, Is.EqualTo(baseType.TypeName));

        }

        [Test]
        public void TrippleInheritWithInterface()
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

            Assert.That(type.Interfaces.Select(r => r.TypeName).Contains(primitiveInterface.TypeName), Is.True);


            Assert.That(type.BaseClass.TypeName, Is.EqualTo(baseType.TypeName));

        }

        [Test]
        public void MultipleInterfaces()
        {
            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(MultipleInterfaces));
           
            Assert.That(type.Interfaces.Count(), Is.EqualTo(3));

        }

        [Test]
        public void MultipleInterfacesNested()
        {
            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(MultipleInterfacesNested));

            Assert.That(type.Interfaces.Count(), Is.EqualTo(2));

        }




    }
}