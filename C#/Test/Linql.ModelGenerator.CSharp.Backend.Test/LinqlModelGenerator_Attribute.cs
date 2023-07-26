using Linql.ModelGenerator.Intermediary;
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
            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(BasicAttribute));
            Assert.That(type is IntermediaryAttribute, Is.True);

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.False);

        }

        [Test]
        public void AttributewithConstructorTest()
        {
            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(AttributeWithConstructor));

            Assert.That(type is IntermediaryAttribute, Is.True);

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.False);

            IntermediaryAttribute attr = type as IntermediaryAttribute;

            Assert.That(attr.Arguments.Count(), Is.EqualTo(2));

        }

        [Test]
        public void AttributewithDefaultsTest()
        {
            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(AttributeWithDefaults));

            Assert.That(type is IntermediaryAttribute, Is.True);

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.False);

            IntermediaryAttribute attr = type as IntermediaryAttribute;

            Assert.That(attr.Arguments.Count(), Is.EqualTo(2));

            IntermediaryArgument arg1 = attr.Arguments[0];
            IntermediaryArgument arg2 = attr.Arguments[1];

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
            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(AttributeInstanceClass));

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.False);

            IntermediaryProperty attrProp = type.Properties[0];

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

            IntermediaryAttributeInstance typeAttr = type.Attributes.FirstOrDefault(r => r.TypeName == nameof(AttributeWithConstructor));
            IntermediaryAttributeInstance propAttr = attrProp.Attributes.FirstOrDefault(r => r.TypeName == nameof(AttributeWithConstructor));

            Assert.That(typeAttr.Arguments[nameof(AttributeWithConstructor.String)], Is.EqualTo("Class"));
            Assert.That(typeAttr.Arguments[nameof(AttributeWithConstructor.Int)], Is.EqualTo(2));

            Assert.That(propAttr.Arguments[nameof(AttributeWithConstructor.String)], Is.EqualTo("Property"));
            Assert.That(propAttr.Arguments[nameof(AttributeWithConstructor.Int)], Is.EqualTo(2));

        }


    }
}