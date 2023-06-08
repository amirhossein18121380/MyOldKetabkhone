using DataModel.Models;

namespace DataAccess.Interface.Book_related;

public interface IUserBookDal
{
    Task<bool> Delete(long Id);
    Task<List<UserBook>> GetAll();
    Task<List<UserBook>> GetAllByUserId(long userid);
    Task<UserBook?> GetById(long id);
    Task<long> Insert(UserBook ub);
    Task<int> Update(UserBook ub);
    Task<List<Book>> GetUserLibrary(long userid);
    Task<List<Book>> GetUserBookMarks(long userid);
    //Task<long> AddToLibrary(UserBook ub);

    Task<UserBook?> GetByBookIdAndUserId(long userid, long bookid);

    Task<UserBook?> GetByBookId(long bookid);
}