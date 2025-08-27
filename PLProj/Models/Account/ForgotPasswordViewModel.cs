using System.ComponentModel.DataAnnotations;

namespace PLProj.Models.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
