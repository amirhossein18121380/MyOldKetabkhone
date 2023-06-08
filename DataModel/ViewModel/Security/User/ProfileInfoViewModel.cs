using System;

namespace DataModel.ViewModel.Security.User
{
    public class ProfileInfoViewModel
    {
        public long UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? NationalCode { get; set; }
        public string UserName { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? MobileNumber { get; set; }
        //public string? CountryCode { get; set; }
        //public string? CountryIso { get; set; }
        public short GenderType { get; set; }
        public string? ProfileImageName { get; set; }
        public long LastBalance { get; set; }
        public int UnreadNotificationCount { get; set; }
        public DateTime? BirthDay { get; set; }
        public string? InviteCode { get; set; }
        public bool? MobileVerified { get; set; }
        public bool EmailVerified { get; set; }
    }
}
