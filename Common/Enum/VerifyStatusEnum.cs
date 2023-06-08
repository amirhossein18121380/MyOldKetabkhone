using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enum;

public enum VerifyStatusEnum
{
    IsOk = 1,
    ValueApproved = 2, //قبلا تایید شده بود
    NewEmailInUse = 3,
    ProblemInSendCode = 4,
    OperationNotAllowed = 5,
    NeWMobileInUse = 6,
    WrongCode = 7
}