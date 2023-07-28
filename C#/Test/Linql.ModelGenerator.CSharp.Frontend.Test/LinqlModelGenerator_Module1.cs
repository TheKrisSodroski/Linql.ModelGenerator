using Linql.ModelGenerator.Core;
using NUnit.Framework;
using System.Text.Json;
using Test.Module1;

namespace Linql.ModelGenerator.CSharp.Frontend.Test
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