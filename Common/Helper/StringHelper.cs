using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helper;

public static class StringHelper
{
    public static string GenarateTrCode()
    {
        return Math.Abs(Guid.NewGuid().GetHashCode()).ToString();
    }

    public static string GenarateRandomNumber(int numberLength)
    {
        Random generator = new Random();
        var period = string.Empty;

        for (int i = 0; i < numberLength; i++)
        {
            period += "9";
        }

        if (string.IsNullOrEmpty(period))
        {
            return string.Empty;
        }

        var registrationCode = generator.Next(1, int.Parse(period)).ToString($"D{numberLength}");

        return registrationCode;
    }

    //public static int GenarateReserveNumber()
    //{
    //    return Math.Abs(Guid.NewGuid().GetHashCode());
    //}

    //public static string GetOperatorName(int operatorId)
    //{
    //    return operatorId == 1 ? "همراه اول" : operatorId == 2 ? "ایرانسل" : "رایتل";
    //}
}
