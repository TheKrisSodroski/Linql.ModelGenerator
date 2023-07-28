using Linql.ModelGenerator.Core;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1.Inheritance;
using Test.Module1;
using Test.Module1.Generics;

namespace Linql.ModelGenerator.CSharp.Backend.Test
{
    public class LinqlModelGenerator_Generic : BaseModelGeneratorTest
    {
        [Test]
        public void GenericWithOne()
        {
            Type genericOne = typeof(GenericOne<object>);
            string typeName = genericOne.Name.Split("`").FirstOrDefault();

            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == typeName);

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.True);

            Assert.That(type.GenericArguments.Count(), Is.EqualTo(1));

            CoreType genericTypeArg = type.GenericArguments.FirstOrDefault();

            Assert.That(genericTypeArg.TypeName, Is.EqualTo("T"));
        }


        [Test]
        public void GenericWithTwo()
        {
            Type genericOne = typeof(GenericTwo<object, object>);
            string typeName = genericOne.Name.Split("`").FirstOrDefault();

            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == typeName);

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.True);

            Assert.That(type.GenericArguments.Count(), Is.EqualTo(2));

            CoreType genericTypeArg = type.GenericArguments.FirstOrDefault();
            CoreType genericTypeArg2 = type.GenericArguments.LastOrDefault();

            Assert.That(genericTypeArg.TypeName, Is.EqualTo("T"));
            Assert.That(genericTypeArg2.TypeName, Is.EqualTo("S"));

        }

        [Test]
        public void GenericWithConstraint()
        {
            Type genericOne = typeof(GenericWithConstraint<IPrimitiveInterface, MultipleInterfacesNested>);
            string typeName = genericOne.Name.Split("`").FirstOrDefault();

            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == typeName);
            CoreType genericConstraint1 = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(IPrimitiveInterface));
            CoreType genericConstraint2 = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(MultipleInterfacesNested));


            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.True);

            Assert.That(type.GenericArguments.Count(), Is.EqualTo(2));

            CoreType genericTypeArg = type.GenericArguments.FirstOrDefault();
            CoreType genericTypeArg2 = type.GenericArguments.LastOrDefault();

            Assert.That(genericTypeArg.TypeName, Is.EqualTo("T"));
            Assert.That(genericTypeArg2.TypeName, Is.EqualTo("S"));

            Assert.That(genericTypeArg.Interfaces.Count(), Is.EqualTo(1));

            Assert.That(genericTypeArg.Interfaces[0].TypeName, Is.EqualTo(genericConstraint1.TypeName));

            Assert.That(genericTypeArg2.Interfaces, Is.EqualTo(null));
            Assert.That(genericTypeArg2.BaseClass.TypeName, Is.EqualTo(genericConstraint2.TypeName));


        }

        [Test]
        public void GenericConstructed()
        {
            Type generic = typeof(GenericConstructed);
            Type genericOne = typeof(GenericOne<object>);
            string typeName = genericOne.Name.Split("`").FirstOrDefault();


            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == generic.Name);
            CoreType genericOneType = this.Module.Types.FirstOrDefault(r => r.TypeName == typeName);


            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.False);

            Assert.That(type.BaseClass.TypeName, Is.EqualTo(genericOneType.TypeName));
            Assert.That(type.BaseClass.GenericArguments.Count(), Is.EqualTo(1));
            Assert.That(type.BaseClass.GenericArguments[0].TypeName, Is.EqualTo("String"));

        }




    }
}