using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helper;

public static class TransactionHelper
{
    public static long GenerateTrCode()
    {
        return Math.Abs(Guid.NewGuid().GetHashCode()) * DateTime.Now.Ticks;
    }

    public static long GenerateReserveNumber()
    {
        var resNum = Math.Abs(Guid.NewGuid().GetHashCode()) * DateTime.Now.Ticks;

        return resNum < 0 ? resNum * -1 : resNum;
    }
}
