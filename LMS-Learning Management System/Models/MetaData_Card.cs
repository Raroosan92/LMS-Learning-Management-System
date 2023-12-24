using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    [ModelMetadataType(typeof(MetaData_Card))]
    public partial class Card
    {

    }

    public class MetaData_Card
    {
        public int Id { get; set; }
        [Display(Name = "رقم البطاقة")]
        public string CardNo { get; set; }
        [Display(Name = "كلمة سر البطاقة")]
        public string CardPassword { get; set; }
        [Display(Name = "سعر البطاقة")]
        public string CardPrice { get; set; }
        [Display(Name = "حالة البطاقة")]
        public bool CardStatus { get; set; }
        [Display(Name = "معرف الطالب")]
        public string UserId { get; set; }
        [Display(Name = "اسم الطالب")]
        public string UserName { get; set; }
        
        [Display(Name = "عدد المواد")]
        public int NumberOfSubjects { get; set; }
        //[Display(Name = "المادة")]
        //public int? SubjectId { get; set; }
        //[Display(Name = "المستوى")]
        //public int? ClassId { get; set; }

        //[Display(Name = "المستوى")]
        //public virtual Class Class { get; set; }
        [NotMapped]
        [Display(Name = "المستوى")]
        public string Classdesc { get; set; }
        //[Display(Name = "المادة")]

        //public virtual Subject Subject { get; set; }
        //[NotMapped]
        //[Display(Name = "المادة")]

        //public string Subjectdesc { get; set; }

        [Display(Name = "اسم الطالب")]
        public virtual AspNetUser User { get; set; }
        [NotMapped]
        [Display(Name = "اسم الطالب")]
        public string Userdesc { get; set; }



    }
}
