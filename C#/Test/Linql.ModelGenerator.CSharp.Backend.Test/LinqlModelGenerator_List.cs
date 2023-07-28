using Linql.ModelGenerator.Core;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1.Inheritance;
using Test.Module1;
using Test.Module1.Generics;
using Test.Module2;

namespace Linql.ModelGenerator.CSharp.Backend.Test
{
    public class LinqlModelGenerator_List : BaseModelGeneratorTest
    {

        [Test]
        public void ListClassTest()
        {
        
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(ListClass));

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.False);
            Assert.That(type.IsIntrinsic, Is.False);

            List<CoreProperty> listProperties = type.Properties.Where(r => r.Type.TypeName == "List").ToList();
            List<CoreProperty> dictionaryProperties = type.Properties.Where(r => r.Type.TypeName == "Dictionary").ToList();
            List<CoreProperty> arrayProperties = type.Properties.Where(r => r.Type.TypeName == "Array").ToList();

            Assert.That(listProperties.Count(), Is.EqualTo(2));
            Assert.That(dictionaryProperties.Count(), Is.EqualTo(1));
            Assert.That(arrayProperties.Count(), Is.EqualTo(1));

            type.Properties.ForEach(r =>
            {
                Assert.That(r.Type.IsIntrinsic, Is.True);

            });
        }
    }
}