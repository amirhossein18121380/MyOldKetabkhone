﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel;

public class LikeViewModel
{
    public long? UserId { get; set; }
    public short? EntityType { get; set; }
    public long? EntityId { get; set; }
}
