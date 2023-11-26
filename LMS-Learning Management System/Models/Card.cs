﻿using System;
using System.Collections.Generic;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class Card
    {
        public int Id { get; set; }
        public string CardNo { get; set; }
        public string CardPassword { get; set; }
        public bool CardStatus { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int SubjectId { get; set; }
        public int ClassId { get; set; }

        public virtual Class Class { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual AspNetUser User { get; set; }
    }
}
