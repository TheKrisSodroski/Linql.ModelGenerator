using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1;

namespace Linql.ModelGenerator.Frontend.Test
{
    public class LinqlModelGenerator_Module2 : BaseModelGeneratorTest
    {

        protected override string ModuleName { get; set; } = "Test.Module2";

        [Test]
        public async Task Generate()
        {
            this.Generator.Generate(this.ModuleJson);
        }
    }
}