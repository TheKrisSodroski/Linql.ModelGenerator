using NUnit.Framework;

namespace Linql.ModelGenerator.Typescript.Frontend.Test
{
    public class LinqlModelGenerator_Module2 : BaseModelGeneratorTest
    {

        protected override string ModuleName { get; set; } = "Test.Module2";

        [Test]
        public async Task Generate()
        {
            this.Generator.Generate();
        }
    }
}