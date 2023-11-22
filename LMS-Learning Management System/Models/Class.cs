using System;
using System.Collections.Generic;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class Class
    {
        public Class()
        {
            Enrollments = new HashSet<Enrollment>();
            Lessons = new HashSet<Lesson>();
        }

        public int Id { get; set; }
        public string Descriptions { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUser { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
