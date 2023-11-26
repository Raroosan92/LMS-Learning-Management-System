using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class Enrollment
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Class Class { get; set; }
        [NotMapped]
        public string Classdesc { get; set; }
        public virtual Subject Subject { get; set; }
        [NotMapped]
        public string Subjectdesc { get; set; }
        public virtual AspNetUser User { get; set; }
        [NotMapped]
        public string Userdesc { get; set; }
    }
}
