using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models;

public class Book
{
    public long Id { get; set; }
    public long? ParentId { get; set; }
    public string? BookName { get; set; }
   // public string? Author { get; set; }
    //public string? Translator { get; set; }
    public string? Publisher { get; set; }
    public DateTime YearOfPublication { get; set; }
    public short BookFormat { get; set; }
    public short BookType { get; set; }
    public int NumberOfPages { get; set; }
    public string? Language { get; set; }
    public long ISBN { get; set; }
    //public string? BookSubject { get; set; }
    public decimal ElectronicVersionPrice { get; set; }
    public string? BookPictureName { get; set; }
    public long? BookPictureId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public bool IsModified { get; set; }
    public long? ModifierId { get; set; }
    public DateTime? LastModified { get; set; }
    public DateTime? CreateOn { get; set; }
}