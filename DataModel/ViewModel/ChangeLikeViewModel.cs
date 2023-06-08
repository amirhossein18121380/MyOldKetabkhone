using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel;

public class ChangeLikeViewModel
{
    public short EntityType { get; set; }
    public long EntityId { get; set; }
    public short Type { get; set; }
}