using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1.Inheritance;
using Test.Module1;
using Test.Module1.Generics;
using Test.Module2;

namespace Linql.ModelGenerator.Backend.Test
{
    public class LinqlModelGenerator_List : BaseModelGeneratorTest
    {
        protected override string ModuleName { get; set; } = "Test.Module2";

        [Test]
        public void ListClassTest()
        {
        
            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(ListClass));

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.False);

            List<IntermediaryProperty> listProperties = type.Properties.Where(r => r.Type.TypeName == "List").ToList();
            List<IntermediaryProperty> dictionaryProperties = type.Properties.Where(r => r.Type.TypeName == "Dictionary").ToList();
            List<IntermediaryProperty> arrayProperties = type.Properties.Where(r => r.Type.TypeName == "Array").ToList();

            Assert.That(listProperties.Count(), Is.EqualTo(2));
            Assert.That(dictionaryProperties.Count(), Is.EqualTo(1));
            Assert.That(arrayProperties.Count(), Is.EqualTo(1));
        }
    }
}