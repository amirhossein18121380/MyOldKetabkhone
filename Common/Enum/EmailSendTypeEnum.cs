using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enum;

public enum EmailSendTypeEnum
{
    All = 0,
    LastLoginDateMoreThan = 1,
    LastLoginDateLower = 2,
    RegisterDateMoreThan = 3,
    RegisterDateLower = 4,
    Withdraw = 5,
    EmailVerified = 6,
    MobileVerified = 7,
    LasBalanceLower = 8,
    LastBalanceMoreThan = 9,
    NewsLetter = 10,
    OutSideUser = 11
}