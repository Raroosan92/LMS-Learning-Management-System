using System.ComponentModel.DataAnnotations;

namespace LMS_Learning_Management_System.Models
{
    public partial  class Login
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public bool Remember { get; set; }
    }
}
