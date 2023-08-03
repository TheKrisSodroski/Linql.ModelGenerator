using Linql.ModelGenerator.CSharp.Backend;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Linql.ComponentModel.Annotations
{
    public class LinqlAnnotationsModuleOverride : IModuleOverridePlugin
    {
        public bool IsValidProperty(Type Type, PropertyInfo PropertyInfo)
        {
            return true;
        }

        public bool IsValidType(Type Type)
        {
            if(Type.Assembly != typeof(KeyAttribute).Assembly)
            {
                return true;
            }
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

        public string ModuleVersionOverride(Assembly Assembly)
        {
            if (Assembly == typeof(KeyAttribute).Assembly)
            {
                AssemblyInformationalVersionAttribute version = Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                string informationVersion = version.InformationalVersion;

                if (informationVersion.Split('.').Count() > 3)
                {
                    informationVersion = String.Join(".", informationVersion.Split('.').Take(3));
                }

                return informationVersion.Split('+')[0] + "-alpha1";
            }
            else
            {
                return null;
            }
        }
    }

}
