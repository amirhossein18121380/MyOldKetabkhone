using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models.Security;

public class Messages
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long? PanelMessageId { get; set; }
    public string? Subject { get; set; }
    public string? MessageContent { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadDate { get; set; }
    public bool IsDeleted { get; set; }
    public long CreatorId { get; set; }
    public DateTime CreateOn { get; set; }
}