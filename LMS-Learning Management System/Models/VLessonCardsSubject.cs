using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class VLessonCardsSubject
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
        public int CardNo { get; set; }
        public decimal? PaymentAmount { get; set; }
        public bool? IsPayment { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string TeacherId { get; set; }

        [NotMapped]
        public string Status2 { get; set; }
        [NotMapped]
        public string Classdesc { get; set; }
        [NotMapped]
        public string Subjectdesc { get; set; }
     
        public int Semester { get; set; }
        public IEnumerable<VTechersInfo> TeacherInfo_Collection { get; set; }
        public IEnumerable<VLessonCardsSubject> VLessonCardsSubject_Collection { get; set; }
        public IEnumerable<VEnrollmentStdDetail> VEnrollmentStdDetailt_Collection { get; set; }

        public static implicit operator VLessonCardsSubject(List<VLessonCardsSubject> v)
        {
            throw new NotImplementedException();
        }
    }
}
