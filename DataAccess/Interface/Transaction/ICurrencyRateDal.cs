using DataModel.Models.Transaction;
using DataModel.ViewModel.Transaction;

namespace DataAccess.Interface.Transaction;

public interface ICurrencyRateDal
{
    Task<bool> Delete(long id);
    Task<List<CurrencyRateViewModel>> GetActiveList();
    Task<CurrencyRate?> GetById(long id);
    Task<List<CurrencyRate>> GetList();
    Task<List<CurrencyRate>> GetListByDestinationCurrencyId(int destinationCurrencyId);
    Task<long> Insert(CurrencyRate entity);
    Task<int> Update(CurrencyRate entity);
}