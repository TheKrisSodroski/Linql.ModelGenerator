using Linql.ModelGenerator.Backend;
using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linql.ModelGenerator.Frontend.Test
{
    public abstract class BaseModelGeneratorTest
    {

        protected string ModelsPath { get; set; } = "../../../../";

        protected virtual string ModuleName { get; set; } = "Test.Module1";

        protected IntermediaryModule Module { get; set; }

        protected string ModuleJson { get; set; }

        protected JsonSerializerOptions JsonOptions { get; set; } = new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

        [OneTimeSetUp]
        public void SetUp()
        {
            LinqlBackendModelGenerator generator = new LinqlBackendModelGenerator(Path.Combine(this.ModelsPath, this.ModuleName));
            this.Module = generator.Generate();
            this.ModuleJson = JsonSerializer.Serialize(this.Module, this.JsonOptions);
        }


    }
}