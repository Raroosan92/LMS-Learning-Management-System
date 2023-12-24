using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_Learning_Management_System.Models
{
    public class Mixed_Enrollments_CardDetails
    {
        //**********************Enrollments***********************
        public int Id { get; set; }
        [Display(Name = "المستوى")]

        public int ClassId { get; set; }
        [Display(Name = "المادة")]

        public int SubjectId { get; set; }
        [Display(Name = "الطالب")]

        public string UserId { get; set; }
        [Display(Name = "تاريخ الانشاء")]

        public DateTime CreatedDate { get; set; }
        [Display(Name = "المستوى")]

        public virtual Class Class { get; set; }
        [Display(Name = "المادة")]
        public virtual Subject Subject { get; set; }
        [Display(Name = "الطالب")]
        public virtual AspNetUser User { get; set; }

        [NotMapped]
        public string Classdesc { get; set; }
        [NotMapped]
        public string Subjectdesc { get; set; }
        [NotMapped]
        public string Userdesc { get; set; }

        [NotMapped]
        public long Card_No { get; set; }
        [NotMapped]
        public string Password_Card { get; set; }

        [NotMapped]
        public string NumberOfSubjects { get; set; }


        [NotMapped]

        public string[] _Classes { get; set; }
        [NotMapped]
        public string[] _Subjecs { get; set; }

        //**********************CardsDetails***********************

        public int Id_CD { get; set; }
        [Display(Name = "رقم البطاقة")]
        public int CardNo { get; set; }
        [Display(Name = "المادة")]
        public int SubjectId_CD { get; set; }
        [Display(Name = "المستوى")]
        public int ClassId_CD { get; set; }

        [Display(Name = "المستوى")]
        public virtual Class Class_CD { get; set; }
        [Display(Name = "الموضوع")]
        public virtual Subject Subject_CD { get; set; }




        public IEnumerable<Card> HD_Collection { get; set; }
        public IEnumerable<CardSubject> DTL_Collection { get; set; }
        public IEnumerable<Subject> Subject_Collection { get; set; }
        public IEnumerable<Class> Class_Collection { get; set; }
    }
}
