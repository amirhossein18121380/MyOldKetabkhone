using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models;

public class Royesh
{
    public long Id { get; set; }
    public long ParentId { get; set; }
    public long UserId { get; set; }
    public int Leaf { get; set; }
    public DateTime StudyTime { get; set; }
    public bool IsPurchase { get; set; }
    public decimal PurchaseAmount { get; set; }
    public long DiscountRefrence { get; set; }
}
