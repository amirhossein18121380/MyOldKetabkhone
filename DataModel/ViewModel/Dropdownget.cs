using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel.Models;

namespace DataModel.ViewModel;

public class Dropdownget
{
    public long AuthorId { get; set; }
    public long TranslatorId { get; set; }
    public long CategoryId { get; set; }
    public long SubjectId { get; set; }
    public string? SubjectTitle { get; set; }
    public string? CategoryTitle { get; set; }
    public string? AuthorFirstName { get; set; }
    public string? TranslatorFirstName { get; set; }
    public string? AuthorLastName { get; set; }
    public string? TranslatorLastName { get; set; }
}
