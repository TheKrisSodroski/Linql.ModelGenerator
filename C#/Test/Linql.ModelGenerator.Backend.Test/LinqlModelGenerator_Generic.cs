using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1.Inheritance;
using Test.Module1;
using Test.Module1.Generics;

namespace Linql.ModelGenerator.Backend.Test
{
    public class LinqlModelGenerator_Generic : BaseModelGeneratorTest
    {
        [Test]
        public void GenericWithOne()
        {
            Type genericOne = typeof(GenericOne<object>);
            string typeName = genericOne.Name.Split("`").FirstOrDefault();

            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == typeName);

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.True);

            Assert.That(type.GenericArguments.Count(), Is.EqualTo(1));

            IntermediaryType genericTypeArg = type.GenericArguments.FirstOrDefault();

            Assert.That(genericTypeArg.TypeName, Is.EqualTo("T"));
        }


        [Test]
        public void GenericWithTwo()
        {
            Type genericOne = typeof(GenericTwo<object, object>);
            string typeName = genericOne.Name.Split("`").FirstOrDefault();

            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == typeName);

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.True);

            Assert.That(type.GenericArguments.Count(), Is.EqualTo(2));

            IntermediaryType genericTypeArg = type.GenericArguments.FirstOrDefault();
            IntermediaryType genericTypeArg2 = type.GenericArguments.LastOrDefault();

            Assert.That(genericTypeArg.TypeName, Is.EqualTo("T"));
            Assert.That(genericTypeArg2.TypeName, Is.EqualTo("S"));

        }

        [Test]
        public void GenericWithConstraint()
        {
            Type genericOne = typeof(GenericWithConstraint<InheritAbstract, MultipleInterfacesNested>);
            string typeName = genericOne.Name.Split("`").FirstOrDefault();

            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == typeName);

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.True);

            Assert.That(type.GenericArguments.Count(), Is.EqualTo(2));

            IntermediaryType genericTypeArg = type.GenericArguments.FirstOrDefault();
            IntermediaryType genericTypeArg2 = type.GenericArguments.LastOrDefault();

            Assert.That(genericTypeArg.TypeName, Is.EqualTo("T"));
            Assert.That(genericTypeArg2.TypeName, Is.EqualTo("S"));

        }


    }
}