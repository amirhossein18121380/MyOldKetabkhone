using DataModel.Models.Transaction;

namespace DataAccess.Interface.Transaction;

public interface ICurrencyDal
{
    Task<bool> Delete(long id);
    Task<Currency?> GetById(long id);
    Task<List<Currency>> GetList();
    Task<long> Insert(Currency entity);
    Task<int> Update(Currency entity);
}