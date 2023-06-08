using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enum;

public enum PaymentMethodEnum
{
    [Description("تیم پشتیبان")]
    FastDeposit = 0,

    [Description("درگاه بانکی")]
    Ipg = 1,

    [Description("رمز ارز")]
    Crypto = 2,

    [Description("کارت به کارت")]
    CardToCard = 3,

    [Description("کارت به کارت")]
    ManualCardToCard = 6,

    [Description("هدیه ثبت نام")]
    RegistrationGift = 7,

    [Description("")]
    GainMoney = 8,

    [Description("")]
    PayPress = 9,

    [Description("هدیه معرفی")]
    InvitationGift = 11,

    [Description("درگاه آنلاین ")]
    SepehrIpg = 12,

    [Description("کارت به کارت اتوماتیک")]
    NightPayCardToCard = 13,

    [Description("کارت به کارت")]
    IrGateCardToCard = 16,
}
