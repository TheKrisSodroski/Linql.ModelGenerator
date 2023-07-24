using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1;

namespace Linql.ModelGenerator.Frontend.Test
{
    public class LinqlModelGenerator_Module1 : BaseModelGeneratorTest
    {

        [Test]
        public async Task Generate()
        {
            this.Generator.Generate();
        }
    }
}