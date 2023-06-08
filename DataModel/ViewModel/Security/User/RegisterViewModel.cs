using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataModel.ViewModel.Security.User
{
    public class RegisterViewModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "نام کاربری حداقل باید 3 کاراکتر باشد")]
        public string? UserName { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        [PasswordPropertyText]
        public string? Password { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Phone]
        public string? MobileNumber { get; set; }

        //[Required]
        //public string? CountryCode { get; set; }

        //public string? NationalCode { get; set; }

       // public string? CountryIso { get; set; }
        public string? InviteCode { get; set; }

        public string? CaptchaId { get; set; }
        public string? CaptchaCode { get; set; }
    }
}