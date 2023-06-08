using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models;

public class Resources
{
    public long Id { get; set; }
    public string? ResourceKey { get; set; }
    public string? ResourceName { get; set; }
    public DateTime CreateOn { get; set; }
}
