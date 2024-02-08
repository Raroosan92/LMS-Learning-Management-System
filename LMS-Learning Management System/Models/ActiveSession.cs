using System;
using System.Collections.Generic;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class ActiveSession
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? LoginDate { get; set; }
        public string DeviceType { get; set; }
        public string MacAddress { get; set; }
        public string ComputerName { get; set; }
    }
}
