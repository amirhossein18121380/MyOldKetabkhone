using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel.Common;

public class ApiSettings
{
    public string? LoginPath { get; set; }
    public string? LogoutPath { get; set; }
    public string? RefreshTokenPath { get; set; }
    public string? AccessTokenObjectKey { get; set; }
    public string? RefreshTokenObjectKey { get; set; }
    public string? AdminRoleName { get; set; }
}