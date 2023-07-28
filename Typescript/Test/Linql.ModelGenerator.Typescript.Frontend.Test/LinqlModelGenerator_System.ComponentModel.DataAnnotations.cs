using Linql.ModelGenerator.CSharp.Backend;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Linql.ModelGenerator.Typescript.Frontend.Test
{
    public class LinqlModelGenerator_SystemComponentDataAnnotation : BaseModelGeneratorTest
    {
        public override void SetUp()
        {
            Type efType = typeof(KeyAttribute);
            LinqlModelGeneratorCSharpBackend generator = new LinqlModelGeneratorCSharpBackend(efType.Assembly);
            generator.ValidTypePlugins.Add(new EFIgnoreTypes());
            this.Module = generator.Generate();
            this.Generator = new LinqlModelGeneratorTypescriptFrontend(this.Module);
        }

        [Test]
        public async Task Generate()
        {
            this.Generator.Generate();
        }
    }

    public class EFIgnoreTypes : IIgnoreTypePlugin
    {
        public bool IsValidProperty(Type Type, PropertyInfo PropertyInfo)
        {
            return true;
        }

        public bool IsValidType(Type Type)
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
            return typesICareAbout.Contains(Type);
        }
    }
}