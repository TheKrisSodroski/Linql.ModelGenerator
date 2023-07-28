using Linql.ModelGenerator.CSharp.Backend;
using Linql.ModelGenerator.Intermediary;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Test.Module1;

namespace Linql.ModelGenerator.CSharp.Frontend.Test
{
    public class LinqlModelGenerator_SystemComponentDataAnnotation : BaseModelGeneratorTest
    {
        public override void SetUp()
        {
            Type efType = typeof(KeyAttribute);
            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(efType.Assembly);
            generator.IgnoreTypePlugins.Add(new EFIgnoreTypes());
            this.Module = generator.Generate();
            this.Generator = new LinqlModelGeneratorCSharpFrontend(this.Module);
        }

        [Test]
        public async Task Generate()
        {
            this.Generator.Generate();
        }
    }

    public class EFIgnoreTypes : IIgnoreTypePlugin
    {
        public bool IgnoreInterface(Type Type)
        {
            return false;
        }

        public bool IgnoreType(Type Type)
        {
            List<Type> typesICareAbout = new List<Type>()
            {
               typeof(KeyAttribute),
               typeof(MaxLengthAttribute),
               typeof(EmailAddressAttribute),
               typeof(MinLengthAttribute),
               typeof(DataType),
               typeof(CreditCardAttribute),
               typeof(PhoneAttribute)
            };
            return !typesICareAbout.Contains(Type);
        }
    }
}