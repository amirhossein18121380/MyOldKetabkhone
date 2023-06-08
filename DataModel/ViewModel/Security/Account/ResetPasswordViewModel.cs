using System.ComponentModel.DataAnnotations;

namespace DataModel.ViewModel.Security.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; } = null!;

        [Required]
        public string ForgetToken { get; set; } = null!;
    }
}