using DataModel.Models;

namespace DataAccess.Interface.Security;

public interface IRoleDal
{
    Task<bool> Delete(long id);
    Task<Role?> GetById(long id);
    Task<List<Role>> GetList();
    Task<long> Insert(Role role);
    Task<int> Update(Role role);
}