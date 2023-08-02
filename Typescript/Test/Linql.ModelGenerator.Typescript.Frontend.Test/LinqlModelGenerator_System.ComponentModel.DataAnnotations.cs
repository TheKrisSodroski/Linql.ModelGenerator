using Linql.ComponentModel.DataAnnotations;
using Linql.ModelGenerator.CSharp.Backend;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Linql.ModelGenerator.Typescript.Frontend.Test
{
    public class LinqlModelGenerator_SystemComponentDataAnnotation : BaseModelGeneratorTest
    {
        public override void SetUp()
        {
            Type efType = typeof(KeyAttribute);
            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(efType.Assembly);
            generator.ValidTypePlugins.Add(new LinqlDataAnnotationsIgnore());
            this.Module = generator.Generate();
            this.Generator = new LinqlModelGeneratorTypescriptFrontend(this.Module);
        }

        [Test]
        public async Task Generate()
        {
            this.Generator.Generate();
        }
    }

}