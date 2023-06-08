using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Common;

public class EmailTemplate
{
    public long Id { get; set; }
    public string? Subject { get; set; }
    public string? EmailContent { get; set; }
    public short SendType { get; set; }
    public bool IsSend { get; set; }
    public long? SenderId { get; set; }
    public DateTime? SendDate { get; set; }
    public string? FilterValue { get; set; }
    public long CreatorId { get; set; }
    public DateTime CreateOn { get; set; }
}