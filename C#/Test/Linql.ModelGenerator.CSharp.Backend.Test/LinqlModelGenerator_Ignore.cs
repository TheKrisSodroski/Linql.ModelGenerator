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

    }
}