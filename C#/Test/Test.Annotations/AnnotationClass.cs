using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Test.Annotations
{
    [Table("annotation." + nameof(AnnotationClass))]
    public class AnnotationClass
    {
        [Key]
        public int AnnotationClassID { get; set; }

        public int AnnotationChildID { get; set; }

        [ForeignKey(nameof(AnnotationClass.AnnotationChildID))]
        public virtual AnnotationChild Child { get; set; }

        [Display]
        public string ShouldNotHaveAnnotation { get; set; }
    }
}
