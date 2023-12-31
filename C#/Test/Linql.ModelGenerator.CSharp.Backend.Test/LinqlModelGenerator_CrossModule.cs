using Linql.ModelGenerator.Core;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1.Inheritance;
using Test.Module1;
using Test.Module1.Generics;
using Test.Module2;

namespace Linql.ModelGenerator.CSharp.Backend.Test
{
    public class LinqlModelGenerator_CrossModule : BaseModelGeneratorTest
    {
        protected override string ModuleName { get; set; } = "Test.Module2";

        [Test]
        public void CrossModuleTest()
        {
        
            CoreType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(CrossModuleClass));

            Assert.That(type, Is.Not.EqualTo(null));
            Assert.That(type.IsClass, Is.True);
            Assert.That(type.IsAbstract, Is.False);
            Assert.That(type.IsInterface, Is.False);
            Assert.That(type.IsGenericType, Is.False);
            Assert.That(type.IsIntrinsic, Is.False);
            Assert.That(type.BaseClass, Is.Not.EqualTo(null));
            Assert.That(type.BaseClass.Properties, Is.EqualTo(null));

        }

        [Test]
        public void CrossModuleToJson()
        {
            Assert.DoesNotThrow(() =>
            {
                string json = JsonSerializer.Serialize(this.Module, this.JsonOptions);
                File.WriteAllText($"{this.Module.ModuleName}.json", json);
            });
        }

    }
}