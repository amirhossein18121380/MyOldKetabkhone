using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel;

public class AccessibleAuthorViewModel
{
    public long AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public bool IsAccess { get; set; }
}
