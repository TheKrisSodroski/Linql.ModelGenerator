using Linql.ModelGenerator.Core;
using NUnit.Framework;
using Test.Module1.Ignore;

namespace Linql.ModelGenerator.CSharp.Backend.Test
{
    public class LinqlModelGenerator_Ignore : BaseModelGeneratorTest
    {

        [Test]
        public void IgnoreClassTest()
        {
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(IgnoreClass));

            Assert.That(type, Is.EqualTo(null));
        }

        [Test]
        public void IgnorePropertyTest()
        {
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(IgnorePropery));

            Assert.That(type, Is.Not.EqualTo(null));

            CoreProperty ignoreProp = type.Properties.FirstOrDefault(r => r.PropertyName == nameof(IgnorePropery.IgnoreProperty));
            CoreProperty notIgnoreProp = type.Properties.FirstOrDefault(r => r.PropertyName == nameof(IgnorePropery.NotIgnoreProperty));

            Assert.That(ignoreProp, Is.EqualTo(null));
            Assert.That(notIgnoreProp, Is.Not.EqualTo(null));

        }

        [Test]
        public void IgnoreClassInGenericTest()
        {
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(IgnoredTypeInGeneric));

            Assert.That(type.BaseClass, Is.Not.EqualTo(null));
            Assert.That(type.BaseClass.GenericArguments.Count, Is.EqualTo(1));

            CoreType genericArg = type.BaseClass.GenericArguments[0];

            Assert.That(genericArg.TypeName, Is.EqualTo("object"));

        }

        [Test]
        public void IgnoreClassInBaseClassTest()
        {
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(IgnoredTypeInBaseClass));

            Assert.That(type, Is.EqualTo(null));

        }
        [Test]
        public void IgnoreAttributeOnClass()
        {
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(IgnoreAttributeOnClass));

            Assert.That(type.Attributes, Is.EqualTo(null));

        }


    }
}