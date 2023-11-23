using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "رقم البطاقة")]
        public string CardNo { get; set; }
        [Display(Name = "كلمة سر البطاقة")]
        public string CardPassword { get; set; }
        [Display(Name = "حالة البطاقة")]
        public bool CardStatus { get; set; }
        [Display(Name = "معرف الطالب")]
        public string UserId { get; set; }
        [Display(Name = "اسم الطالب")]
        public string UserName { get; set; }

        public virtual AspNetUser User { get; set; }
    }
}
