using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Annotations
{
    [Table("annotation." + nameof(AnnotationChild))]
    public class AnnotationChild
    {
    }
}
