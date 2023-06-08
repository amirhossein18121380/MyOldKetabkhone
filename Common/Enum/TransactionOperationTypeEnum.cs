using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enum;

public enum TransactionOperationTypeEnum
{
    [Description("افزایش موجودی")]
    IncreaseWallet = 1,

    [Description("کاهش موجودی")]
    DecreaseWallet = 2,

    [Description("برداشت از حساب")]
    Withdrawal = 4
}
