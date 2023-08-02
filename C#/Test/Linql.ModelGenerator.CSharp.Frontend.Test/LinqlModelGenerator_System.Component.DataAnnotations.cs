using Linql.ModelGenerator.CSharp.Backend;
using Linql.ModelGenerator.Core;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Test.Module1;
using System.Reflection;
using Linql.ComponentModel.DataAnnotations;

namespace Linql.ModelGenerator.CSharp.Frontend.Test
{
    public class LinqlModelGenerator_SystemComponentDataAnnotation : BaseModelGeneratorTest
    {
        public override void SetUp()
        {
            Type efType = typeof(KeyAttribute);
            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(efType.Assembly);
            generator.OverridePlugins.Add(new LinqlDataAnnotationsIgnore());
            this.Module = generator.Generate();
            this.Generator = new LinqlModelGeneratorCSharpFrontend(this.Module);
        }

        [Test]
        public async Task Generate()
        {
            this.Generator.Generate();
        }
    }

}