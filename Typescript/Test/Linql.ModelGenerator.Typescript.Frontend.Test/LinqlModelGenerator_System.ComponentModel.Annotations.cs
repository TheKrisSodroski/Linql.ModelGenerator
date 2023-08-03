using Linql.ComponentModel.Annotations;
using Linql.ModelGenerator.CSharp.Backend;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace Linql.ModelGenerator.Typescript.Frontend.Test
{
    public class LinqlModelGenerator_SystemComponentModelAnnotations : BaseModelGeneratorTest
    {
        public override void SetUp()
        {
            Type efType = typeof(KeyAttribute);
            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(efType.Assembly);
            generator.OverridePlugins.Add(new LinqlAnnotationsModuleOverride());
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