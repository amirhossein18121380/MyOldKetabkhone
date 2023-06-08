using DataModel.Models;

namespace DataAccess.Interface;

public interface IBookCategoryDal
{
    Task<bool> Delete(long Id);
    Task<List<BookCategory>> GetAll();
    Task<bool> DeleteByBookId(long bookId);
    Task<BookCategory?> GetById(long id);
    Task<long> Insert(BookCategory bookCategory);
    Task<int> Update(BookCategory bookCategory);
}