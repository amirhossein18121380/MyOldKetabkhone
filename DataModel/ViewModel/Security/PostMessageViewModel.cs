using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel.Security;

public class PostMessageViewModel
{
    public long UserId { get; set; }
    public string? Subject { get; set; }
    public string? MessageContent { get; set; }
}