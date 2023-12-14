using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS_Learning_Management_System.Models
{
    public class User
    {
        [Required]
        [Display(Name = "الاسم")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "الايميل")]

        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "كلمة المرور")]

        public string Password { get; set; }

        [Required]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }

        public string UserType { get; set; }

        public string UserTypeDesc { get; set; }
        [Display(Name = "تاريخ االميلاد")]
        [Required]
        public DateTime BirthDate { get; set; }
        [Display(Name = "الدولة")]
        [Required]
        public string Country { get; set; }
        [Display(Name = "الاسم الكامل")]
        [Required]
        public string FullName { get; set; }

        //[NotMapped]
        //[Display(Name = "المحاضر")]
        //public string UserId { get; set; }

        //[NotMapped]
        //[Display(Name = "اسم المحاضر")]
        //public string UserName { get; set; }

        [NotMapped]
        [Display(Name = "الموضوع")]
        public int SubjectId { get; set; }


        [NotMapped]
        [Display(Name = "المستوى")]
        public int ClassId { get; set; }


        [NotMapped]
        [Display(Name = "نوع المستخدم")]
        public string RoleId { get; set; }

        public DateTime CreatedDateTime { get; set; }

    }
}
