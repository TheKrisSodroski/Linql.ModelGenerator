using Linql.ModelGenerator.CSharp.Backend;
using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linql.ModelGenerator.Typescript.Frontend.Test
{
    public abstract class BaseModelGeneratorTest
    {

        protected string ModelsPath { get; set; } = "../../../../../../C#/Test";

        protected virtual string ModuleName { get; set; } = "Test.Module1";

        protected IntermediaryModule Module { get; set; }

        protected string ModuleJson { get; set; }

        protected JsonSerializerOptions JsonOptions { get; set; } = new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

        protected LinqlModelGeneratorTypescriptFrontend Generator { get; set; }

        protected bool Clean { get; set; } = false;

        [OneTimeSetUp]
        public void SetUp()
        {
            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(Path.Combine(this.ModelsPath, this.ModuleName));
            this.Module = generator.Generate();
            this.ModuleJson = JsonSerializer.Serialize(this.Module, this.JsonOptions);
            this.Generator = new LinqlModelGeneratorTypescriptFrontend(this.ModuleJson);

            List<IntermediaryType> anyCasts = new List<IntermediaryType>();
            IntermediaryType geo = new IntermediaryType() { TypeName = "Geography", Module = "System.Spatial" };
            IntermediaryType geometry = new IntermediaryType() { TypeName = "Geometry", Module = "System.Spatial" };
            anyCasts.AddRange(
                new List<IntermediaryType>() 
                { 
                    geo, 
                    geometry 
                });
            this.Generator.AnyCasts = anyCasts;
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