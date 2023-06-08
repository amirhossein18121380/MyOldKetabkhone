using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel;

public class TranslatorVm
{
    public long Id { get; set; }
    public string? TranslatorFirstName { get; set; }
    public string? TranslatorLastName { get; set; }
    public DateTime Birthday { get; set; }
    public string? Email { get; set; }
    public string? Country { get; set; }
    public string? Language { get; set; }
    public string? Bio { get; set; }
}
