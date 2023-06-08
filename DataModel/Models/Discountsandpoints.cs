using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models;

public class Discountsandpoints
{
    public long Id { get; set; }
    public string? DiscountCoupon { get; set; }
    public int PercentAmount { get; set; }
    public bool IsCredit { get; set; }
    public DateTime DiscountActivationTime { get; set; }
    public DateTime DiscountExpirationTime { get; set; }
    public long BookId { get; set; }
    public long UserId { get; set; }
}
