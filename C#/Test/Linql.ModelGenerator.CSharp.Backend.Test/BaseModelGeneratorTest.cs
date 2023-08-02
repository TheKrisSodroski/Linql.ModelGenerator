using Linql.ModelGenerator.Core;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linql.ModelGenerator.CSharp.Backend.Test
{
    public abstract class BaseModelGeneratorTest
    {

        protected string ModelsPath { get; set; } = "../../../../";

        protected virtual string ModuleName { get; set; } = "Test.Module1";

        protected CoreModule Module { get; set; }

        protected JsonSerializerOptions JsonOptions { get; set; } = new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

        [OneTimeSetUp]
        public virtual void SetUp()
        {
            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(Path.Combine(this.ModelsPath, this.ModuleName));
            this.Module = generator.Generate();
        }


    }
}