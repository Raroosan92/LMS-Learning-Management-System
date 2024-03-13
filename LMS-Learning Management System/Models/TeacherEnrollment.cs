using System;
using System.Collections.Generic;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class TeacherEnrollment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }

    }
}
