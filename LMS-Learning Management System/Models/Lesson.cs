using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class Lesson
    {
        public Lesson()
        {
            Documents = new HashSet<Document>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlVideo { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public bool? Status { get; set; }
        public string CreatedUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string TeacherId { get; set; }
        public int? Semester { get; set; }

        [NotMapped]
        public string Status2 { get; set; }
        public virtual Class Class { get; set; }
        [NotMapped]
        public string Classdesc { get; set; }
        public virtual Subject Subject { get; set; }
        [NotMapped]
        public string Subjectdesc { get; set; }

        [NotMapped]
        public string SemesterDesc { get; set; }

        public virtual ICollection<Document> Documents { get; set; }

    }
}
