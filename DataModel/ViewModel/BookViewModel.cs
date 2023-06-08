using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel.Models;

namespace DataModel.ViewModel;

public class BookViewModel
{
    //public long? ParentId { get; set; }
    public string? BookName { get; set; }
    public string? Publisher { get; set; }
    public DateTime YearOfPublication { get; set; }
    public short BookFormat { get; set; }
    public short BookType { get; set; }
    public int NumberOfPages { get; set; }
    public string? Language { get; set; }
    public long ISBN { get; set; }
    public decimal ElectronicVersionPrice { get; set; }
    public string? BookPictureName { get; set; }
    //public long BookPictureId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public bool IsModified { get; set; }
    //public long? ModifierId { get; set; }
    public DateTime? LastModified { get; set; }
    public DateTime? CreateOn { get; set; }
    public List<Dropdownget>? Author { get; set; }
    public List<Dropdownget>? Translator { get; set; }
    public List<Dropdownget>? Subjects { get; set; }
    public List<Dropdownget>? Category { get; set; }

}
