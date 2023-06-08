using DataModel.Models.Transaction;

namespace DataAccess.Interface.Transaction;

public interface IWalletDal
{
    Task<Wallet?> GetById(long id);
    string GetInsertQuery();
    string GetLastBalanceUpdateQuery();
    Task<long> GetUserLastBalance(long userId);
    Task<Wallet?> GetUserMainWallet(long userId);
    Task<long> Insert(Wallet entity);
    Task<long> UpdateLastBalance(long id, decimal lastBalance);
}