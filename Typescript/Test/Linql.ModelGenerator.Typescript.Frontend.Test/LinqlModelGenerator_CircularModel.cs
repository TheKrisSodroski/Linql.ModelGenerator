using NUnit.Framework;

namespace Linql.ModelGenerator.Typescript.Frontend.Test
{
    public class LinqlModelGenerator_CircularModel : BaseModelGeneratorTest
    {

        protected override string ModuleName { get; set; } = "Test.CircularModel";

        [Test]
        public async Task Generate()
        {
            this.Generator.Generate();
        }
    }
}