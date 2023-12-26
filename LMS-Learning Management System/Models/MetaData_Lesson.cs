using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    [ModelMetadataType(typeof(MetaData_Lesson))]
    public partial class Lesson
    {
    }


    public class MetaData_Lesson
    {
      
        public int Id { get; set; }
        [Display(Name = "اسم الدرس")]

        public string Name { get; set; }
        [Display(Name = "وصف الدرس")]

        public string Description { get; set; }
        [Display(Name = "رابط الفيديو")]

        public string UrlVideo { get; set; }
        [Display(Name = "المستوى")]

        public int ClassId { get; set; }
        [Display(Name = "المادة")]

        public int SubjectId { get; set; }
        [Display(Name = "الحالة")]

        public bool? Status { get; set; }
        [Display(Name = "اسم المُنشئ")]

        public string CreatedUser { get; set; }
        [Display(Name = "تاريخ الاضافة")]

        public DateTime? CreatedDate { get; set; }
        [Display(Name = "المستوى")]

        public virtual Class Class { get; set; }
        [Display(Name = "المادة")]
        public virtual Subject Subject { get; set; }
        
        [Display(Name = "معلم المادة")]
        [Required]
        public string TeacherID { get; set; }
    }
}
