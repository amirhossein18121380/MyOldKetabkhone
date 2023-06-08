using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel.Security;

public class NotificationViewModel
{
    public long Id { get; set; }
    public string? Subject { get; set; }
    public int UnreadCount { get; set; }
}
