using DataModel.Models;

namespace DataAccess.Interface.Security;

public interface IRoleMemberDal
{
    Task<bool> Delete(long id);
    Task<bool> DeleteUserRole(long userId);
    Task<RoleMember?> GetById(long id);
    Task<List<RoleMember>> GetByUserId(long userId);
    Task<List<RoleMember>> GetList();
    Task<long> Insert(RoleMember roleMember);
    Task<int> Update(RoleMember entity);
}