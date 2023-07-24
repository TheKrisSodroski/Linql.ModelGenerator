using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linql.ModelGenerator.CSharp.Backend.Test
{
    public class LinqlModelGenerator_Constructor : BaseModelGeneratorTest
    {

        [Test]
        public void AssemblyFromCsProj()
        {
            Assert.DoesNotThrow(() =>
            {
                LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(Path.Combine(this.ModelsPath, "Test.Module1"));
            });
           
        }

        [Test]
        public void AssemblyFromDll()
        {
            Assert.DoesNotThrow(() =>
            {
                LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(Path.Combine(this.ModelsPath, "Test.Module1", "bin", "Debug", "netstandard2.0", "Test.Module1.dll"));
            });

        }

        [Test]
        public void AssemblyFromDllDoesNotExist()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(Path.Combine(this.ModelsPath, "Test.Module1", "bin", "Debug", "netstandard2.0", "Test.Module.dll"));
            });

        }

        [Test]
        public void CannotFindCsProj()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(Path.Combine(this.ModelsPath, "Test.Module1", "bin"));
            });

        }

        [Test]
        public void ModuleToJson()
        {
            Assert.DoesNotThrow(() =>
            {
                string json = JsonSerializer.Serialize(this.Module, this.JsonOptions);
                File.WriteAllText($"{this.Module.ModuleName}.json", json);
            });
        }




    }
}