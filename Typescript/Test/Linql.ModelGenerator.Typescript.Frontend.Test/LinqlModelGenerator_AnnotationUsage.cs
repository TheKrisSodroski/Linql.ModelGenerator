using Linql.ComponentModel.Annotations;
using Linql.ModelGenerator.Core;
using Linql.ModelGenerator.CSharp.Backend;
using Linql.System.Spatial;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace Linql.ModelGenerator.Typescript.Frontend.Test
{
    public class LinqlModelGenerator_AnnotationUsageTest : BaseModelGeneratorTest
    {
        protected override string ModuleName { get; set; } = "Test.Annotations";

        protected CoreModule EFModule { get; set; }
        public override void SetUp()
        {
            Type efType = typeof(KeyAttribute);
            LinqlModelGeneratorCSharpBackend efGenerator = new LinqlModelGeneratorCSharpBackend(efType.Assembly);
            efGenerator.OverridePlugins.Add(new LinqlAnnotationsModuleOverride());
            efGenerator.OverridePlugins.Add(new LinqlSpatialModuleOverride());
            this.EFModule = efGenerator.Generate();
            base.SetUp();
        }

        [Test]
        public async Task Generate()
        {
            this.Generator.Generate();
        }
    }

}