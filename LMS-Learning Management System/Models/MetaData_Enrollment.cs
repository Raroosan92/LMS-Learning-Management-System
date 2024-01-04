using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    [ModelMetadataType(typeof(MetaData_Enrollment))]
    public partial class Enrollment
    {
    }

    public class MetaData_Enrollment
    {
     
        public int Id { get; set; }
        [Display(Name = "المستوى")]

        public int ClassId { get; set; }
        [Display(Name = "المادة")]

        public int SubjectId { get; set; }
        [Display(Name = "الطالب")]

        public string UserId { get; set; }
        [Display(Name = "تاريخ الانشاء")]

        public DateTime CreatedDate { get; set; }
        [Display(Name = "المستوى")]

        public virtual Class Class { get; set; }
        [Display(Name = "المادة")]
        public virtual Subject Subject { get; set; }
        [Display(Name = "الطالب")]
        public virtual AspNetUser User { get; set; }

        [Display(Name = "المعلم")]

        public string TeacherId { get; set; }

        [Required]
        [Display(Name = "الفصل")]
        public int Semester { get; set; }

    }
}
