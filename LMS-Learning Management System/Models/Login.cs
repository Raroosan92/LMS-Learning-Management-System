using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [NotMapped]
        public string FullName { get; set; }
        
        [NotMapped]
        public string devicetype { get; set; }   
        [NotMapped]
        public string clientIpAddress { get; set; }

        public bool Remember { get; set; }

        public string SessionId { get; set; }
        public string SecurityStamp { get; set; }

    }
}
