using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel;

public class UpdateTranslatorInfoViewModel
{
    public long TranslatorId { get; set; }
    public string? TranslatorFirstName { get; set; } = null!;
    public string? TranslatorLastName { get; set; } = null!;
    public DateTime Birthday { get; set; }
    public string? Country { get; set; } = null!;
    public string? Language { get; set; } = null!;
    public string? Bio { get; set; } = null!;
}
