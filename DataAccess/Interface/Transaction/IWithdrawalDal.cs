using DataModel.Models.Transaction;
using DataModel.ViewModel.Transaction;

namespace DataAccess.Interface.Transaction;

public interface IWithdrawalDal
{
    Task<(long id, long lastBalance)> AddWithdrawal(Withdrawal entity, long walletId);
    Task<bool> Delete(long id);
    Task<Withdrawal?> GetById(long id);
    string GetInsertQuery();
    Task<(List<GetWithdrawalListViewModel> data, int totalCount)> GetList(WithdrawalGetListFilterViewModel filterModel);
    Task<int> GetUnseenWithdrawalCount();
    string GetUpdateQuery();
    Task<long> Insert(Withdrawal entity);
    Task<long> ReverseWithdrawal(Withdrawal entity);
    Task<int> Update(Withdrawal entity);
    Task<long> VerifyWithdrawal(Withdrawal entity);
}