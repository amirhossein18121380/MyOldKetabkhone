using DataModel.ViewModel.Role;

namespace DataModel.ViewModel.Security.User;

public class UserViewModel
{
    public long UserId { get; set; }
    public string? Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? NationalCode { get; set; }
    public string? ProfilePictureName { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? MobileNumber { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryIso { get; set; }
    public short GenderType { get; set; }
    public bool IsActive { get; set; }
    public bool IsBane { get; set; }
    public short ChatStatus { get; set; }
    public bool IsPanelUser { get; set; }
    public List<AccessibleRoleViewModel>? Roles { get; set; }
    public DateTime? BirthDay { get; set; }
}

