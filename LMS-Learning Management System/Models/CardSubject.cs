using System;
using System.Collections.Generic;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class CardSubject
    {
        public int Id { get; set; }
        public int CardNo { get; set; }
        public int SubjectId { get; set; }
        public int ClassId { get; set; }

        public virtual Class Class { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual Card Card { get; set; }
    }
}
