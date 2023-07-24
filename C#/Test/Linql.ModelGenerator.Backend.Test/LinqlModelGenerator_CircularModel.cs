using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;
using Test.CircularModel;

namespace Linql.ModelGenerator.Backend.Test
{
    public class LinqlModelGenerator_CircularModel : BaseModelGeneratorTest
    {
        protected override string ModuleName { get; set; } = "Test.CircularModel";

        [Test]
        public void CircularModelTest()
        {
        
            IntermediaryType type = this.Module.Types.FirstOrDefault(r => r.TypeName == nameof(CircularModel1));

         
        }

        [Test]
        public void CircularModelToJson()
        {
            Assert.DoesNotThrow(() =>
            {
                string json = JsonSerializer.Serialize(this.Module, this.JsonOptions);
                File.WriteAllText($"{this.Module.ModuleName}.json", json);
            });
        }

    }
}