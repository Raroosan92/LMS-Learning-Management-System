using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_Learning_Management_System.Models
{
    public class Mixed_Cards_CardDetails
    {
        public int Id { get; set; }
        [Display(Name = "رقم البطاقة")]
        public string CardNo { get; set; }
        [Display(Name = "كلمة سر البطاقة")]
        public string CardPassword { get; set; }
        [Display(Name = "سعر البطاقة")]
        public string CardPrice { get; set; }
        [Display(Name = "حالة البطاقة")]
        public bool CardStatus { get; set; }
        [Display(Name = "معرف الطالب")]
        public string UserId { get; set; }
        [Display(Name = "اسم الطالب")]
        public string UserName { get; set; }
        
        [Display(Name = "عدد موادالبطاقة")]
        public int NumberOfSubjects { get; set; }
        //[Display(Name = "المادة")]
        //public int? SubjectId { get; set; }
        //[Display(Name = "المستوى")]
        //public int? ClassId { get; set; }

        //[Display(Name = "المستوى")]
        //public virtual Class Class { get; set; }
        [NotMapped]
        [Display(Name = "المستوى")]
        public string Classdesc { get; set; }
        //[Display(Name = "المادة")]

        //public virtual Subject Subject { get; set; }
        //[NotMapped]
        //[Display(Name = "المادة")]

        //public string Subjectdesc { get; set; }

        [Display(Name = "اسم الطالب")]
        public virtual AspNetUser User { get; set; }
        [NotMapped]
        [Display(Name = "اسم الطالب")]
        public string Userdesc { get; set; }

        public int Id_details { get; set; }
        [Display(Name = "رقم البطاقة")]
        public int CardNo_details { get; set; }
        [Display(Name = "المادة")]
        public int SubjectId { get; set; }
        [Display(Name = "المستوى")]
        public int ClassId { get; set; }

        [Display(Name = "المستوى")]
        public virtual Class Class { get; set; }
        [Display(Name = "الموضوع")]
        public virtual Subject Subject { get; set; }
        [Display(Name = "المعلم")]

        public string TeacherId { get; set; }
        [Display(Name = "القيمة")]

        public double? PaymentAmount { get; set; }
        [Display(Name = "مدفوع؟")]

        public bool? IsPayment { get; set; }
        [Display(Name = "تاريخ الدفع")]
        public DateTime? PaymentDate { get; set; }
        [Required]
        [Display(Name = "الفصل")]
        public int Semester { get; set; }
        public IEnumerable<Card> HD_Collection { get; set; }
        public IEnumerable<CardSubject> DTL_Collection { get; set; }
        public IEnumerable<Subject> Subject_Collection { get; set; }
        public IEnumerable<Class> Class_Collection { get; set; }
    }
}
