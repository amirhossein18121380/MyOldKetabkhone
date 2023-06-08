namespace DataModel.ViewModel.Security.Account
{
    public class LoginViewModel
    {
        public string UserName { set; get; } = null!;
        public string Password { set; get; } = null!;
        public string? CaptchaId { get; set; }
        public string? CaptchaCode { get; set; }
    }
}
