using System;
using System.Collections.Generic;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class TeacherSalesCard
    {
        public int CardNo { get; set; }
        public string CardPrice { get; set; }
        public int? NumberOfSubjects { get; set; }
        public int? TeacherCardPrice { get; set; }
        public string UserName { get; set; }
        public string UserTypeDesc { get; set; }
        public string TeacherName { get; set; }
        public string Subject { get; set; }
        public string Class { get; set; }
        public string Country { get; set; }
        public string TeacherId { get; set; }
        public double? PaymentAmount { get; set; }
        public bool? IsPayment { get; set; }
        public string StudentName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int CardSer { get; set; }
        public string TeacherUserID { get; set; }
    }
}
