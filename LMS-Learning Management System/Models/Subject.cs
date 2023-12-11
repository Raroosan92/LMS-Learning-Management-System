using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class Subject
    {
        public Subject()
        {
            CardSubjects = new HashSet<CardSubject>();
            Enrollments = new HashSet<Enrollment>();
            Lessons = new HashSet<Lesson>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public bool? Status { get; set; }
        [NotMapped]
        public string Status2 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUser { get; set; }

        public virtual ICollection<CardSubject> CardSubjects { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
