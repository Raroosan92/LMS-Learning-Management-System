using System;
using System.Collections.Generic;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class VLessonsDesc
    {
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
        public string SemesterDesc { get; set; }
        public int? Semester { get; set; }
        public string SubjectDesc { get; set; }
        public string ClassDesc { get; set; }
        public string StatusDesc { get; set; }
    }
}
