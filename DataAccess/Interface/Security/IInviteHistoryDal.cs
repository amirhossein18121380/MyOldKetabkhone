using DataModel.Models.Security;
using PokerNet.DataModel.ViewModel.Security;

namespace DataAccess.Interface.Security;

public interface IInviteHistoryDal
{
    Task<bool> Delete(long id);
    Task<int> GetGiftCount(long parentUserId);
    Task<InviteHistory?> GetHistoryRecord(long userId);
    Task<(List<InviteHistoryViewModel>? data, int totalCount)> GetListByFilter(InviteHistoryFilterViewModel filterModel);
    Task<long> Insert(InviteHistory entity);
    Task<int> Update(InviteHistory entity);
}