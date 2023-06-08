namespace DataModel.Models;

public class User
{
    public long Id { get; set; }
    public long? ParentId { get; set; }
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public short GenderType { get; set; }
    public string MobileNumber { get; set; } = null!;
    public bool MobileVerified { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryIso { get; set; }
    public string? UniqCode { get; set; }
    public string Email { get; set; } = null!;
   public string? NationalCode { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime? BirthDay { get; set; }
    public long? ProfilePictureId { get; set; }
    public string? ProfilePictureName { get; set; }
    public bool IsActive { get; set; }
    public bool IsBane { get; set; }
    public short ChatStatus { get; set; }
    public bool IsPanelUser { get; set; }
    public string? ForgetPasswordToken { get; set; }
    public DateTime? ForgetPasswordTokenExpiration { get; set; }
    public string? EmailVerifyCode { get; set; }
    public DateTime? EmailVerifiedDate { get; set; }
    public string? MobileVerifyCode { get; set; }
    public DateTime? MobileVerifiedDate { get; set; }
   // public bool NewsLetter { get; set; }
    public string? RegistrationIp { get; set; }
    public string? Comment { get; set; }
    public DateTime? LastLoggedIn { get; set; }
    public long CreatorId { get; set; }
    public DateTime CreateOn { get; set; }
    public string? GetDisplayName()
    {
        var dpName = !string.IsNullOrEmpty(DisplayName)
            ? DisplayName
            : $"{FirstName} {LastName}".Trim().Length > 0
                ? $"{FirstName} {LastName}"
                : UserName;

        return dpName;
    }
}