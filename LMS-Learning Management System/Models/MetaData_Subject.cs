using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    [ModelMetadataType(typeof(MetaData_Subject))]
    public partial class Subject
    {
    }


    public class MetaData_Subject
    {
        public MetaData_Subject()
        {
            Enrollments = new HashSet<Enrollment>();
            Lessons = new HashSet<Lesson>();
        }

        public int Id { get; set; }
        [Display(Name = "اسم المادة بالتفصيل")]

        public string Name { get; set; }
        [Display(Name = "الاسم المختصر")]

        public string Abbreviation { get; set; }
        [Display(Name = "الحالة")]

        public bool? Status { get; set; }
        [Display(Name = "تاريخ الاضافة")]

        public DateTime? CreatedDate { get; set; }
        [Display(Name = "اسم المُنشئ")]
        public string CreatedUser { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
