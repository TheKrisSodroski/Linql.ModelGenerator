using NUnit.Framework;
using System.Text.Json;

namespace Linql.ModelGenerator.Backend.Test
{
    public class LinqlModelGenerator_Constructor : BaseModelGeneratorTest
    {

        [Test]
        public void AssemblyFromCsProj()
        {
            Assert.DoesNotThrow(() =>
            {
                LinqlModelGenerator generator = new LinqlModelGenerator(Path.Combine(this.ModelsPath, "Test.Module1"));
            });
           
        }

        [Test]
        public void AssemblyFromDll()
        {
            Assert.DoesNotThrow(() =>
            {
                LinqlModelGenerator generator = new LinqlModelGenerator(Path.Combine(this.ModelsPath, "Test.Module1", "bin", "Debug", "netstandard2.0", "Test.Module1.dll"));
            });

        }

        [Test]
        public void AssemblyFromDllDoesNotExist()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                LinqlModelGenerator generator = new LinqlModelGenerator(Path.Combine(this.ModelsPath, "Test.Module1", "bin", "Debug", "netstandard2.0", "Test.Module.dll"));
            });

        }

        [Test]
        public void CannotFindCsProj()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                LinqlModelGenerator generator = new LinqlModelGenerator(Path.Combine(this.ModelsPath, "Test.Module1", "bin"));
            });

        }



    }
}