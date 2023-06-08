using System.Collections.Generic;
using DataModel.ViewModel.Security.User;


namespace DataModel.ViewModel.Security.Account
{
    public class LoginResultViewModel
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public ProfileInfoViewModel? ProfileInfo { get; set; }
    }

    public class PanelLoginResultViewModel
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public long[]? ResourceIds { get; set; }
        public PanelInfoViewModel? ProfileInfo { get; set; }
    }

    public class PanelInfoViewModel
    {
        public long UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfileImageName { get; set; }
        public int UnseenWithdrawalCount { get; set; }
        public int UnseenSuggestionCount { get; set; }
    }
}
