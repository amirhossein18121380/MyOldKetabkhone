using DataModel.Models;

namespace DataAccess.Interface;

public interface IAuthorBookDal
{
    Task<bool> Delete(long Id);
    Task<List<AuthorBook>> GetAll();
    Task<AuthorBook?> GetById(long id);
    Task<bool> DeleteByBookId(long bookId);
    Task<long> Insert(AuthorBook authorBook);
    Task<int> Update(AuthorBook authorBook);
}