using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel;

public class ReviewUpdateViewModel
{
    public long Id { get; set; }
    //public long UserId { get; set; }
    //public short EntityType { get; set; }
    public short EntityId { get; set; }
    public string? CommentValue { get; set; }
    //public DateTime CommentDate { get; set; }
}
