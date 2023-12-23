using Microsoft.AspNetCore.Identity;
using System;

namespace LMS_Learning_Management_System.Models
{
    public class AppUser : IdentityUser
    {


        public string UserType { get; set; }
        public string UserTypeDesc { get; set; }

        public DateTime BirthDate { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public string Country { get; set; }

        public string SecurityStamp { get; set; }

    }
}
