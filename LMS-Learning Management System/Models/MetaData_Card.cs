using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    [ModelMetadataType(typeof(MetaData_Card))]
    public partial class Card
    {

    }

    public class MetaData_Card
    {
        public int Id { get; set; }
        public string CardNo { get; set; }
        public string CardPassword { get; set; }
        public bool CardStatus { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public virtual AspNetUser User { get; set; }
    }
}
