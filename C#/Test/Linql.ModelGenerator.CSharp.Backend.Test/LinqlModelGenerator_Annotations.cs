using Linql.ComponentModel.Annotations;
using Linql.ModelGenerator.Core;
using Linql.System.Spatial;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace Linql.ModelGenerator.CSharp.Backend.Test
{
    public class LinqlModelGenerator_Annotations : BaseModelGeneratorTest
    {
        protected override string ModuleName { get; set; } = "Test.Annotations";
        public override void SetUp()
        {
            LinqlModelGeneratorCSharpBackend annotationsGen = new LinqlModelGeneratorCSharpBackend(typeof(KeyAttribute).Assembly);
            annotationsGen.OverridePlugins.Add(new LinqlAnnotationsModuleOverride());
            annotationsGen.OverridePlugins.Add(new LinqlSpatialModuleOverride());

            CoreModule annotations = annotationsGen.Generate();

            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(Path.Combine(this.ModelsPath, this.ModuleName));
            generator.OverridePlugins.Add(new LinqlAnnotationsModuleOverride());
            generator.OverridePlugins.Add(new LinqlSpatialModuleOverride());

            this.Module = generator.Generate();

        }


        [Test]
        public void ShouldGenerate()
        {
            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(typeof(KeyAttribute).Assembly);
            this.Module = generator.Generate();
        }

    }
}