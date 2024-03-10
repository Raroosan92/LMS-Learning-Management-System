using System;
using System.Collections.Generic;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class Document
    {
        public long Id { get; set; }
        public int LessonId { get; set; }
        public string WorkingSheets { get; set; }
        public string Booklets { get; set; }

        public virtual Lesson Lesson { get; set; }
    }
}
