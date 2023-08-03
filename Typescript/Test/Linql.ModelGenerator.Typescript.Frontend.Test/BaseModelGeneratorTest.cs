using Linql.ComponentModel.Annotations;
using Linql.ModelGenerator.Core;
using Linql.ModelGenerator.CSharp.Backend;
using Linql.System.Spatial;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linql.ModelGenerator.Typescript.Frontend.Test
{
    public abstract class BaseModelGeneratorTest
    {

        protected string ModelsPath { get; set; } = "../../../../../../C#/Test";

        protected virtual string ModuleName { get; set; } = "Test.Module1";

        protected CoreModule Module { get; set; }

        protected string ModuleJson { get; set; }

        protected JsonSerializerOptions JsonOptions { get; set; } = new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

        protected LinqlModelGeneratorTypescriptFrontend Generator { get; set; }

        protected bool Clean { get; set; } = false;

        [OneTimeSetUp]
        public virtual void SetUp()
        {
            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(Path.Combine(this.ModelsPath, this.ModuleName));
            generator.OverridePlugins.Add(new LinqlAnnotationsModuleOverride());
            generator.OverridePlugins.Add(new LinqlSpatialModuleOverride());
            this.Module = generator.Generate();
            this.ModuleJson = JsonSerializer.Serialize(this.Module, this.JsonOptions);
            this.Generator = new LinqlModelGeneratorTypescriptFrontend(this.ModuleJson);

            List<CoreType> anyCasts = new List<CoreType>();
            CoreType geo = new CoreType() { TypeName = "Geography", Module = "System.Spatial" };
            CoreType geometry = new CoreType() { TypeName = "Geometry", Module = "System.Spatial" };
            anyCasts.AddRange(
                new List<CoreType>() 
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