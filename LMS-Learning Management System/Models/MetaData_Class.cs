using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    [ModelMetadataType(typeof(MetaData_Class))]
    public partial class Class
    {
    }

    public class MetaData_Class
    {
        public MetaData_Class()
        {
            Cards = new HashSet<Card>();
            Enrollments = new HashSet<Enrollment>();
            Lessons = new HashSet<Lesson>();
        }


        public int Id { get; set; }
        [Display(Name = "اسم المستوى")]
        public string Descriptions { get; set; }
        [Display(Name = "الحالة")]
        public bool? Status { get; set; }
        [Display(Name = "تاريخ الانشاء")]
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "اسم المُنشئ")]
        public string CreatedUser { get; set; }

        public virtual ICollection<Card> Cards { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
