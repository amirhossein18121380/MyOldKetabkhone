using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models;

public class Rate
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public short EntityType { get; set; }
    public long EntityId { get; set; }
    public short RateValue { get; set; }
    public DateTime CreateOn { get; set; }
}
