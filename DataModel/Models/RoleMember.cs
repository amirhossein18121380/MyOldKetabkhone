﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models;

public class RoleMember
{
    public long Id { get; set; }
    public long RoleId { get; set; }
    public long UserId { get; set; }
    public long CreatorId { get; set; }
    public DateTime CreateOn { get; set; }
}