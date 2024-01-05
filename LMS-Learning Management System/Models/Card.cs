using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace LMS_Learning_Management_System.Models
{

        [Index(nameof(CardNo), IsUnique = true)]
    public partial class Card
    {
        public Card()
        {
            CardSubjects = new HashSet<CardSubject>();
        }
        public int Id { get; set; }

        public int CardNo { get; set; }
        public string CardPassword { get; set; }
        public string CardPrice { get; set; }
        public bool CardStatus { get; set; }
        [NotMapped]

        public string Status2 { get; set; }
        public string UserId { get; set; }
        public int NumberOfSubjects { get; set; }
        public string UserName { get; set; }
        [NotMapped]
        public string Classdesc { get; set; }
        [NotMapped]
        public string Subjectdesc { get; set; }
        public virtual AspNetUser User { get; set; }
        [NotMapped]
        public string Userdesc { get; set; }

        public virtual ICollection<CardSubject> CardSubjects { get; set; }

    }
}
