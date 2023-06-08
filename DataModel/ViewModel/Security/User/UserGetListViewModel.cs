using System;

namespace DataModel.ViewModel.Security.User
{
    public class UserGetListViewModel
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DisplayName { get; set; }
        public short GenderType { get; set; }
        public string? MobileNumber { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryIso { get; set; }
        public string? Email { get; set; }
        public string? NationalCode { get; set; }
        public bool IsActive { get; set; }
        public string? ProfilePictureName { get; set; }
        public bool IsBane { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public DateTime RegisterDate { get; set; }
        public long LastBalance { get; set; }
        public string? RegistrationIp { get; set; }
    }
}