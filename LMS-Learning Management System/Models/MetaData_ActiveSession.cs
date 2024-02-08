using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    [ModelMetadataType(typeof(MetaData_ActiveSession))]
    public partial class ActiveSession
    {
    }

    public  class MetaData_ActiveSession
    {
        public int Id { get; set; }
        [Display(Name = "المعرف")]
        public string UserId { get; set; }
        [Display(Name = "اسم المستخدم")]
        public string UserName { get; set; }
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }
        [Display(Name = "اول تسجيل دخول")]
        public DateTime? LoginDate { get; set; }
        [Display(Name = "نوع الجهاز")]
        public string DeviceType { get; set; }
        [Display(Name = "MAC Address")]
        public string MacAddress { get; set; }
        [Display(Name = "اسم الجهاز")]
        public string ComputerName { get; set; }
    }
   
}
