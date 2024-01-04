using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_Learning_Management_System.Models
{
    public class Mixed_VTeacher_Sales_Cards
    {

        public int CardNo { get; set; }
        public string CardPrice { get; set; }
        public int? NumberOfSubjects { get; set; }
        public decimal? TeacherCardPrice { get; set; }
        public decimal? CenterCardPrice { get; set; }
        public string UserName { get; set; }
        public string UserTypeDesc { get; set; }
        public string TeacherName { get; set; }
        public string Subject { get; set; }
        public string Class { get; set; }
        public string Country { get; set; }
        public string TeacherId { get; set; }
        public decimal? PaymentAmount { get; set; }
        public bool? IsPayment { get; set; }
        public string StudentName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int CardSer { get; set; }
        public string TeacherUserID { get; set; }

        public int Semester { get; set; }
        [NotMapped]
        public string SemesterDesc { get; set; }


        [NotMapped]
        public DateTime? startDate { get; set; }
        [NotMapped]
        public DateTime? endDate { get; set; }

        [NotMapped]
        public int? teacherName { get; set; }


        [NotMapped]
        public int? subject1 { get; set; }

        public IEnumerable<TeacherSalesCard> Teacher_Sales_Cards_Collection { get; set; }
    }
}
