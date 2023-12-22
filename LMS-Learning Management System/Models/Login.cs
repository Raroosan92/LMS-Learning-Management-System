using System.ComponentModel.DataAnnotations;

namespace LMS_Learning_Management_System.Models
{
    public partial  class Login
    {
        [Required]
        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public bool Remember { get; set; }

        public string SessionId { get; set; }
        public string SecurityStamp { get; set; }

    }
}
