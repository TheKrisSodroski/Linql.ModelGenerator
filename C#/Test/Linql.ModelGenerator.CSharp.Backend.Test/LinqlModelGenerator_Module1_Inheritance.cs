using Linql.ModelGenerator.Core;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1;
using Test.Module1.Inheritance;

namespace Linql.ModelGenerator.CSharp.Backend.Test
{
    public class LinqlModelGenerator_Module1_Inheritance : BaseModelGeneratorTest
    {

        [Test]
        public void InheritedAbstract()
        {
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(InheritAbstract));
            CoreType baseType = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(PrimitiveAbstract));

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
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(DoubleInheritAbstract));
            CoreType baseType = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(InheritAbstract));

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
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(TripleInheritWithInterface));
            CoreType baseType = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(DoubleInheritAbstract));
            CoreType primitiveInterface = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(IPrimitiveInterface));

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
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(MultipleInterfaces));
           
            Assert.That(type.Interfaces.Count(), Is.EqualTo(3));

        }

        [Test]
        public void MultipleInterfacesNested()
        {
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(MultipleInterfacesNested));

            Assert.That(type.Interfaces.Count(), Is.EqualTo(2));

        }




    }
}