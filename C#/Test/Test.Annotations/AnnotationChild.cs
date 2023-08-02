using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Test.Annotations
{
    [Table("annotation." + nameof(AnnotationChild))]
    public class AnnotationChild
    {
        [CreditCard]
        public string CreditCard { get; set; }

        //[CreditCard("Error Message", "Error Message Resource Name")]
        public string CreditCardArgs { get; set; }

        [Display]
        public string DisplayNoArgs { get; set; }

        [Display(AutoGenerateField = true, GroupName = "GroupName")]
        public string DisplayArgs { get; set; }
    }
}
