using System;

namespace DataModel.ViewModel.Security.User
{
    public class UserGetListFilterViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public long? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? NationalCode { get; set; }
        public string? DisplayName { get; set; }
        public string? RegistrationIp { get; set; }
        public string? MobileNumber { get; set; }
        public DateTime? RegisterDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsBane { get; set; }
        public long? Balance { get; set; }
    }
}
