using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    [ModelMetadataType(typeof(MetaData_CardSubject))]
    public partial class CardSubject
    {
    }

    public class MetaData_CardSubject
    {

        public int Id { get; set; }
        [Display(Name = "رقم البطاقة")]
        public int CardNo { get; set; }
        [Display(Name = "المادة")]
        public int SubjectId { get; set; }
        [Display(Name = "المستوى")]
        public int ClassId { get; set; }

        [Display(Name = "المستوى")]
        public virtual Class Class { get; set; }
        [Display(Name = "الموضوع")]
        public virtual Subject Subject { get; set; }
    }
 
}
