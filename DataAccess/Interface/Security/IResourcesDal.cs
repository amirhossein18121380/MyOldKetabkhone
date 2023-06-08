using DataModel.Models;

namespace DataAccess.Interface.Security;

public interface IResourcesDal
{
    Task<bool> Delete(long id);
    Task<Resources?> GetById(long id);
    Task<List<Resources>> GetList();
    Task<long> Insert(Resources entity);
    Task<int> Update(Resources entity);
}