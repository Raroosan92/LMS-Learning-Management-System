using System;
using System.Collections.Generic;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class VLessonsDocument
    {
        public string WorkingSheets { get; set; }
        public string Booklets { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Descriptions { get; set; }
        public int SubjectId { get; set; }
        public int ClassId { get; set; }
        public long DocId { get; set; }
        public int? Semester { get; set; }
    }
}
