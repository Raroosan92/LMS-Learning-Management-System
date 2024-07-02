using System;
using System.Collections.Generic;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class VCardsDetail
    {
        public int Id { get; set; }
        public int CardNo { get; set; }
        public string CardPassword { get; set; }
        public string CardPrice { get; set; }
        public string Status2 { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int? NumberOfSubjects { get; set; }
        public string Userdesc { get; set; }
    }
}
