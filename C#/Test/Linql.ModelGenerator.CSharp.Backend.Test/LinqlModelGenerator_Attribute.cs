using Linql.ModelGenerator.Core;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1.Inheritance;
using Test.Module1;
using Test.Module1.Generics;
using Test.Module1.Attributes;

namespace Linql.ModelGenerator.CSharp.Backend.Test
{
    public class LinqlModelGenerator_Attribute : BaseModelGeneratorTest
    {
        [Test]
        public void BasicAttributeTest()
        {
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(BasicAttribute));
            Assert.That(type is CoreAttribute, Is.True);

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.False);

        }

        [Test]
        public void AttributewithConstructorTest()
        {
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(AttributeWithConstructor));

            Assert.That(type is CoreAttribute, Is.True);

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.False);

            CoreAttribute attr = type as CoreAttribute;

            Assert.That(attr.RequiredArguments.Count(), Is.EqualTo(2));

        }

        [Test]
        public void AttributewithDefaultsTest()
        {
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(AttributeWithDefaults));

            Assert.That(type is CoreAttribute, Is.True);

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.False);

            CoreAttribute attr = type as CoreAttribute;

            Assert.That(attr.RequiredArguments.Count(), Is.EqualTo(2));

            CoreArgument arg1 = attr.RequiredArguments[0];
            CoreArgument arg2 = attr.RequiredArguments[1];

            Assert.That(arg1.DefaultValue, Is.EqualTo("String"));
            Assert.That(arg1.ArgumentName, Is.EqualTo("String"));
            Assert.That(arg2.DefaultValue, Is.EqualTo(5));
            Assert.That(arg2.ArgumentName, Is.EqualTo("Int"));

        }

        [Test]
        public void AttributeInstanceTest()
        {
            List<string> attributes = new List<string>()
            {
                nameof(BasicAttribute),
                nameof(AttributeWithConstructor),
                nameof(AttributeWithDefaults)
            };
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(AttributeInstanceClass));

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.False);

            CoreProperty attrProp = type.Properties[0];

            Assert.That(type.Attributes.Count(), Is.EqualTo(3));
            Assert.That(attrProp.Attributes.Count(), Is.EqualTo(3));

            type.Attributes.ForEach(r =>
            {
                Assert.That(attributes.Contains(r.TypeName), Is.True);
            });

            attrProp.Attributes.ForEach(r =>
            {
                Assert.That(attributes.Contains(r.TypeName), Is.True);
            });

            CoreAttributeInstance typeAttr = type.Attributes.FirstOrDefault(r => r.TypeName == nameof(AttributeWithConstructor));
            CoreAttributeInstance propAttr = attrProp.Attributes.FirstOrDefault(r => r.TypeName == nameof(AttributeWithConstructor));

            Assert.That(typeAttr.Arguments[nameof(AttributeWithConstructor.String)], Is.EqualTo("Class"));
            Assert.That(typeAttr.Arguments[nameof(AttributeWithConstructor.Int)], Is.EqualTo(2));

            Assert.That(propAttr.Arguments[nameof(AttributeWithConstructor.String)], Is.EqualTo("Property"));
            Assert.That(propAttr.Arguments[nameof(AttributeWithConstructor.Int)], Is.EqualTo(2));

        }


    }
}