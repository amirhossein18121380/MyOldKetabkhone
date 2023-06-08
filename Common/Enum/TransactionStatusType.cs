using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enum;

public enum TransactionStatusType
{
    [Description("ناقص")]
    Reserve = 1,

    [Description("موفق")]
    IsOk = 2,

    [Description("برگشتی")]
    Reverse = 3,

    [Description("نا موفق")]
    Failed = 4,

    [Description("در حال بررسی")]
    Pending = 5
}
