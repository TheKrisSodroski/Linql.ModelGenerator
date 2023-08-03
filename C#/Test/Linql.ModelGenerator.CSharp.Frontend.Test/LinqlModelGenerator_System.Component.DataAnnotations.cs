using Linql.ComponentModel.Annotations;
using Linql.ModelGenerator.CSharp.Backend;
using Linql.System.Spatial;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace Linql.ModelGenerator.CSharp.Frontend.Test
{
    public class LinqlModelGenerator_SystemComponentDataAnnotation : BaseModelGeneratorTest
    {
        public override void SetUp()
        {
            Type efType = typeof(KeyAttribute);
            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(efType.Assembly);
            generator.OverridePlugins.Add(new LinqlAnnotationsModuleOverride());
            generator.OverridePlugins.Add(new LinqlSpatialModuleOverride());
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