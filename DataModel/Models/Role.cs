using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models;

public class Role
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Comment { get; set; }
    public long CreatorId { get; set; }
    public DateTime CreateOn { get; set; }
}
