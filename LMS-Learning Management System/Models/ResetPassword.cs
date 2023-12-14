using System.ComponentModel.DataAnnotations;

namespace LMS_Learning_Management_System.Models
{
    public class ResetPassword
    {
        [Required]
        public string Password { get; set; }

        [Compare("كلمة المرور", ErrorMessage = "كلمة المرور غير مطابقة !")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
    }
}
