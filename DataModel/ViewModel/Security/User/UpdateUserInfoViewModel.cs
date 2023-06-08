using System;

namespace DataModel.ViewModel.Security.User
{
    public class UpdateUserInfoViewModel
    {
        public long UserId { get; set; }
        public string Password { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; } = null!;
        public string? NationalCode { get; set; }
        public string UserName { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string MobileNumber { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
        public string CountryIso { get; set; } = null!;
        public short GenderType { get; set; }
        public bool IsActive { get; set; }
        public bool IsBane { get; set; }
        public short ChatStatus { get; set; }
        public bool IsPanelUser { get; set; }
        public long[] RoleIds { get; set; } = null!;
        public DateTime? BirthDay { get; set; }
    }
}
