using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class VEnrollmentStdDetail
    {
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
        public string Descriptions { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int EnrollmentClassId { get; set; }
        public int EnrollmentSubjectId { get; set; }
        public string TeacherId { get; set; }
        public int CardNo { get; set; }
        public bool Subjects_Status { get; set; }
        public bool Classes_Status { get; set; }




        public IEnumerable<VTechersInfo> TeacherInfo_Collection { get; set; }
        public IEnumerable<VEnrollmentStdDetail> VEnrollmentStdDetailt_Collection { get; set; }
    }
}
