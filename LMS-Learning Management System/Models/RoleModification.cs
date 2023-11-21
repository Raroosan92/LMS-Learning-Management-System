using System.ComponentModel.DataAnnotations;

namespace LMS_Learning_Management_System.Models
{
    public class RoleModification
    {
        [Required]
        public string RoleName { get; set; }

        public string RoleId { get; set; }
        
        public string[]? AddIds { get; set; }
        
        public string[]? DeleteIds { get; set; }
    }
}
