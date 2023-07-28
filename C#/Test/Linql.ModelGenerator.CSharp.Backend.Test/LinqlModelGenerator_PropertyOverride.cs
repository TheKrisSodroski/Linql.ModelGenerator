using Linql.ModelGenerator.Core;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1.Inheritance;
using Test.Module1;
using Test.Module1.Generics;
using Test.Module2;
using Test.Module1.PropertyOverride;

namespace Linql.ModelGenerator.CSharp.Backend.Test
{
    public class LinqlModelGenerator_PropertyOverride : BaseModelGeneratorTest
    {

        [Test]
        public void PropertyOverrideTest()
        {
        
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(PropertyOverride));

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.False);
            Assert.That(type.IsIntrinsic, Is.False);

            CoreProperty overriden = type.Properties.First(r => r.PropertyName == nameof(PropertyOverride.Integer));
            CoreProperty notOverriden = type.Properties.First(r => r.PropertyName == nameof(PropertyOverride.NotOverride));

            Assert.That(overriden.Overriden, Is.EqualTo(true));
            Assert.That(overriden.Virtual, Is.EqualTo(true));
            Assert.That(notOverriden.Overriden, Is.EqualTo(false));
            Assert.That(notOverriden.Virtual, Is.EqualTo(false));

        }
    }
}