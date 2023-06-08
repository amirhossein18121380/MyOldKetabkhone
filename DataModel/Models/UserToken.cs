using System;

namespace DataModel.Models;

public class UserToken
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string AccessTokenHash { get; set; }
    public DateTime AccessTokenExpiresDateTime { get; set; }
    public string RefreshTokenIdHash { get; set; }
    public DateTime RefreshTokenExpiresDateTime { get; set; }
    public DateTime CreateOn { get; set; }
}

