using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enum;

public enum WithdrawalStatusTypeEnum
{
    [Description("User Requested")]
    UserRequested = 0,

    [Description("Pending")]
    Pending = 1,

    [Description("Success")]
    Success = 2,

    [Description("Fail")]
    Fail = 3,

    [Description("Cancel By User")]
    CancelByUser = 4,

    [Description("Verify And Success")]
    VerifyAndSuccess = 5,
}