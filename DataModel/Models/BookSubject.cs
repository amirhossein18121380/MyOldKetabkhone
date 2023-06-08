using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models;

public class BookSubject
{
    public long Id { get; set; }
    public long BookId { get; set; }
    public long SubjectId { get; set; }
}
