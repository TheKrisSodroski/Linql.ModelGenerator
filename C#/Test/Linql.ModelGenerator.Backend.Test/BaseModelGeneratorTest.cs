using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;

namespace Linql.ModelGenerator.Backend.Test
{
    public abstract class BaseModelGeneratorTest
    {

        protected string ModelsPath { get; set; } = "../../../../";

        protected IntermediaryModule Module { get; set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            LinqlModelGenerator generator = new LinqlModelGenerator(Path.Combine(this.ModelsPath, "Test.Module1"));
            this.Module = generator.Generate();
        }


    }
}