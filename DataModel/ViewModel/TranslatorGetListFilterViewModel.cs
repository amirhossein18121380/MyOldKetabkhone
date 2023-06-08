using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel;

public class TranslatorGetListFilterViewModel
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public long? TranslatorId { get; set; }
    public string? TranslatorFirstName { get; set; }
    public string? TranslatorLastName { get; set; }
    public DateTime? Birthday { get; set; }
    public string? Country { get; set; }
    public string? Language { get; set; }
    public int? Age { get; set; }
}