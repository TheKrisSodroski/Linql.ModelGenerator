using Linql.ComponentModel.DataAnnotations;
using Linql.ModelGenerator.Core;
using Linql.ModelGenerator.CSharp.Backend;
using Linql.ModelGenerator.Typescript.Frontend.Test;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

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
            efGenerator.OverridePlugins.Add(new LinqlDataAnnotationsIgnore());
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