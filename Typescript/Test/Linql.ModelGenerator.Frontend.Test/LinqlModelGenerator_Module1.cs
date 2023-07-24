using NUnit.Framework;

namespace Linql.ModelGenerator.Typescript.Frontend.Test
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