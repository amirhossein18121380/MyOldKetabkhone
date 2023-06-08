using DataModel.Models;
using DataModel.ViewModel;

namespace DataAccess.Interface;

public interface IBookDal
{
    Task<long> AddBook(Book book);
    Task<List<Book>> GetAllPublishers();
    Task<List<Author>> GetAuthorsByBookId(long id);
    //Task<List<long>> GetAuthorsidByBookId(long id);
    Task<Book?> GetBookById(long id);
    Task<List<Book>> GetByAuthor(string author);
    Task<List<Book>> GetByBookName(string bookName);
    Task<Book?> GetByBookPictureId(long bookPictureId);
    Task<List<Book>> GetByBookSubject(string subject);
    Task<Book?> GetByISBN(long isbn);
    Task<List<Book>> GetByPublisher(string publisher);
    Task<List<Book>> GetByTranslator(string translator);
    Task<List<Category>> GetCategoryByBookId(long id);
    Task<(List<BookListViewModel> data, int totalCount)> GetList(BookGetListFilterViewModel filterModel);
    Task<List<Subjects>> GetSubjectsByBookId(long id);
    Task<int> GetTheAverageRateOfBookById(long id);
    Task<List<Translator>> GetTranslatorByBookId(long id);
    Task<int> UpdateBook(Book book);
}