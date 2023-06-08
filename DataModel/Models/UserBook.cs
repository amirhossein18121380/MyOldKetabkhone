using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models;

public class UserBook
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long BookId { get; set; }
    public bool IsMarked { get; set; }
    public bool IsAdded { get; set; }
    public bool IsPurchase { get; set; }
    public DateTime CreateOn { get; set; }
}