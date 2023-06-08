namespace DataModel.ViewModel.Security.User
{
    public class ChangePasswordViewModel
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
