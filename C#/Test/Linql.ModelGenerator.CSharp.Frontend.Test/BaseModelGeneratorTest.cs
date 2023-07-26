using Linql.ModelGenerator.CSharp.Backend;
using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linql.ModelGenerator.CSharp.Frontend.Test
{
    public abstract class BaseModelGeneratorTest
    {

        protected string ModelsPath { get; set; } = "../../../../";

        protected virtual string ModuleName { get; set; } = "Test.Module1";

        protected IntermediaryModule Module { get; set; }

        protected string ModuleJson { get; set; }

        protected JsonSerializerOptions JsonOptions { get; set; } = new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

        protected LinqlModelGeneratorCSharpFrontend Generator { get; set; }

        protected bool Clean { get; set; } = false;

        [OneTimeSetUp]
        public void SetUp()
        {
            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(Path.Combine(this.ModelsPath, this.ModuleName));
            this.Module = generator.Generate();
            this.ModuleJson = JsonSerializer.Serialize(this.Module, this.JsonOptions);
            this.Generator = new LinqlModelGeneratorCSharpFrontend(this.ModuleJson);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (this.Clean)
            {
                this.Generator.Clean();
            }
        }


    }
}