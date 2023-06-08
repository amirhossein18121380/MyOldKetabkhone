using DataModel.Models;
using DataModel.ViewModel;

namespace DataAccess.Interface;

public interface IAuthorDal
{
    Task<bool> Delete(long Id);
    Task<(List<AuthorViewModel> data, int totalCount)> GetList(AuthorGetListFilterViewModel filterModel);
    Task<List<Author>> GetAll();
    Task<List<AuthorProfileViewModel>> GetALlAuthorProfile();
    Task<AuthorProfileViewModel?> GetAuthorProfileById(long authorId);
    //Task<Author?> GetByEmail(string? email);
    Task<Author?> GetByFirstnameAndLastname(string? authorfirstname, string? authorlastname);
    Task<Author?> GetById(long id);
    Task<long> Insert(Author author);
    Task<int> Update(Author author);
    Task<List<Book>> BooksByAuthorId(long id);
    Task<List<AuthorScore>> GetLightprofile();
    Task<AuthorScore?> GetlightprofileById(long id);
}