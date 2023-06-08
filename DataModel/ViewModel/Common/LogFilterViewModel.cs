using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel.Common;

public class LogFilterViewModel
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public short? Level { get; set; }
    public string? MethodName { get; set; }
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
