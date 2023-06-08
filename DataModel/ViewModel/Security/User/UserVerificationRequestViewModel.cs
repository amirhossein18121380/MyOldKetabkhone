using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel.Security.User;

public class UserVerificationRequestViewModel
{
    /// <summary>
    /// 1: Email, 2:MobileNumber
    /// </summary>
    public short VerifyType { get; set; }
    public string? VerifyValue { get; set; }
}

public class UserVerificationViewModel
{
    public short VerifyType { get; set; }
    public string? Code { get; set; }
}