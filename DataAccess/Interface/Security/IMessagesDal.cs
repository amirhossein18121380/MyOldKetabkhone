using DataModel.Models.Security;

namespace DataAccess.Interface.Security;

public interface IMessagesDal
{
    Task<bool> Delete(long id);
    Task<Messages?> GetById(long id);
    Task<List<Messages>> GetList();
    Task<(List<Messages> data, int totalCount)> GetListByUserId(long userId, int pageSize, int pageNumber);
    Task<int> GetUserUnreadMessageCount(long userId);
    Task<long> Insert(Messages entity);
    Task<int> Update(Messages entity);
    Task<bool> UpdateIsDeleted(long id);
    Task<bool> UpdateMessageIsRead(long id);
}