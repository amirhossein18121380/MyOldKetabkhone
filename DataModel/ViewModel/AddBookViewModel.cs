using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModel;

public class AddBookViewModel
{
    //public long Id { get; set; }
    //public long AuthorId { get; set; }
    //public long TranslatorId { get; set; }
    //public long SubjectId { get; set; }
    //public long CategoryId { get; set; }

    //public long? ParentId { get; set; }
    public string? BookName { get; set; } = null!;
    //public string? Author { get; set; }
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
    //public string? BookPictureName { get; set; }
    //public long BookPictureId { get; set; }
    public bool IsActive { get; set; }
    //public DateTime? CreateOn { get; set; }

    public long[] authorids { get; set; } = null!;
    public long[] translatorids { get; set; } = null!;
    public long[] booksubjectids { get; set; } = null!;
    public long[] bookcategoryids { get; set; } = null!;

    //public List<AccessibleAuthorViewModel>? Authors { get; set; }
    //public List<AccessibleRoleViewModel>? Translator { get; set; }
    //public List<AccessibleRoleViewModel>? BookSubjects { get; set; }
    //public List<AccessibleRoleViewModel>? BookCategories { get; set; }
}