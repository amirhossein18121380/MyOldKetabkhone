using DataModel.Models;

namespace DataAccess.Interface.Security;

public interface IRolePolicyDal
{
    Task<bool> Delete(long id);
    Task<List<RolePolicy>> GetByRoleId(long roleId);
    string GetInsertQuery();
    Task<List<RolePolicy>> GetList();
    Task<RolePolicy?> GetList(long id);
    Task<long[]> GetResourceIdsByUserId(long userId);
    Task<long> Insert(RolePolicy entity);
    Task<bool> SetRolePolicy(long roleId, long[] recourseIds, long creatorId);
    Task<int> Update(RolePolicy entity);
}