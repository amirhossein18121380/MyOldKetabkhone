﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel;

public class AuthorProfileViewModel
{
    public long Id { get; set; }
    public string? AuthorFirstName { get; set; }
    public string? AuthorLastName { get; set; }
    public string? ProfilePictureName { get; set; }
    public DateTime Birthday { get; set; }
    public string? Country { get; set; }
    public string? Language { get; set; }
    public string? Bio { get; set; }
    public int Age { get; set; }
    public short AverageRate { get; set; }
    public int Likes { get; set; }
    public int DisLikes { get; set; }
}