using System.ComponentModel.DataAnnotations;

namespace LMS_Learning_Management_System.Models
{
    public class TwoFactor
    {
        [Required]
        public string TwoFactorCode { get; set; }
    }
}
