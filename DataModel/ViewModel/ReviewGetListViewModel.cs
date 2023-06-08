using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel;

public class ReviewGetListViewModel
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public long? ParentId { get; set; }
    public long? UserId { get; set; }
    public short EntityType { get; set; }
    public long? EntityId { get; set; }
    public DateTime? CommentDate { get; set; }
}
