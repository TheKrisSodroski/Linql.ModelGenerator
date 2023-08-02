using Linql.ModelGenerator.CSharp.Backend;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Linql.ComponentModel.DataAnnotations
{
    public class LinqlDataAnnotationsIgnore : IIgnoreTypePlugin
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
               typeof(PhoneAttribute),
               typeof(ForeignKeyAttribute),
               typeof(ColumnAttribute),
               typeof(InversePropertyAttribute),
               typeof(TableAttribute),
               typeof(RequiredAttribute),
               typeof(RangeAttribute),
               typeof(RegularExpressionAttribute),
               typeof(DisplayAttribute)
            };
            return typesICareAbout.Contains(Type);
        }

        public bool IsObjectType(Type Type)
        {
            return false;
        }

    }

}
