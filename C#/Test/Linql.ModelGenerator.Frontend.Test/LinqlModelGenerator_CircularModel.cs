using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1;

namespace Linql.ModelGenerator.Frontend.Test
{
    public class LinqlModelGenerator_CircularModel : BaseModelGeneratorTest
    {

        protected override string ModuleName { get; set; } = "Test.CircularModel";

        [Test]
        public async Task Generate()
        {
            this.Generator.Generate();
        }
    }
}