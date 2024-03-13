using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

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


        [NotMapped]
        public List<SelectListItem> ESelectedClasses { get; set; }
        [NotMapped]
        public List<SelectListItem> ESelectedSubjectes { get; set; }


        [NotMapped]
        public int?[] SelectedClasses { get; set; }
        [NotMapped]
        public int?[] SelectedSubjectes { get; set; }


    }
}
