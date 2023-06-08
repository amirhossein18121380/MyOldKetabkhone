using DataModel.Models;

namespace DataAccess.Interface;

public interface IBookSubjectDal
{
    Task<bool> Delete(long Id);
    Task<List<BookSubject>> GetAll();
    Task<bool> DeleteByBookId(long bookId);
    Task<BookSubject?> GetById(long id);
    Task<long> Insert(BookSubject bookCategory);
    Task<int> Update(BookSubject bookCategory);
}