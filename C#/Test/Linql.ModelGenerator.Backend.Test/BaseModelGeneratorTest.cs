using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;

namespace Linql.ModelGenerator.Backend.Test
{
    public abstract class BaseModelGeneratorTest
    {

        protected string ModelsPath { get; set; } = "../../../../";

        protected virtual string ModuleName { get; set; } = "Test.Module1";

        protected IntermediaryModule Module { get; set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            LinqlModelGenerator generator = new LinqlModelGenerator(Path.Combine(this.ModelsPath, this.ModuleName));
            this.Module = generator.Generate();
        }


    }
}