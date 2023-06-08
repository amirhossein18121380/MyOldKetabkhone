using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models;

public class Review
{
    public long Id { get; set; }
    public long? ParentId { get; set; }
    public long UserId { get; set; }
    public short EntityType { get; set; }
    public long EntityId { get; set; }
    public string CommentValue { get; set; } = null!;
    public DateTime CommentDate { get; set; }
}