using DataModel.Models.Transaction;

namespace DataAccess.Interface.Transaction;

public interface IDepositRequestDal
{
    string GetInsertQuery();
    Task<List<DepositRequest>> GetList();
    Task<long> InsertRequest(DepositRequest entity);
}