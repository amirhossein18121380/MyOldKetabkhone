using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel.Security.User;

public class UserVerificationRequestResultViewModel
{
    public long UserId { get; set; }
    public short Status { get; set; }
    public string? Message { get; set; }
}
