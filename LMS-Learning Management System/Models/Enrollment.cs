using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class Enrollment
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public string TeacherId { get; set; }
        public string UserId { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:MM:ss tt}")]

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

        [NotMapped]
        public long Card_No { get; set; }
        [NotMapped]
        public string Password_Card { get; set; }
        
        [NotMapped]
        public string NumberOfSubjects { get; set; }


        [NotMapped]

        public string []_Classes { get; set; }
        [NotMapped]
        public string [] _Subjecs { get; set; }
    }
}
