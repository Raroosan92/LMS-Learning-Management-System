using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LMS_Learning_Management_System.Models
{

    [ModelMetadataType(typeof(MetaData_Login))]
    public partial class Login
    {
    }


    public partial class MetaData_Login
    {
        [Display(Name = "الايميل")]

        [Required]
        public string Email { get; set; }
        [Display(Name = "كلمة المرور")]

        [Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
        [Display(Name = "تذكرني")]

        public bool Remember { get; set; }



    }


}
