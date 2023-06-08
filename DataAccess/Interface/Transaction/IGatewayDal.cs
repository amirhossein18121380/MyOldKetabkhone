using DataModel.Models.Transaction;
using DataModel.ViewModel.Transaction;

namespace DataAccess.Interface.Transaction;

public interface IGatewayDal
{
    Task<bool> Delete(long id);
    Task<List<GatewayCurrencyViewModel>> GetActiveList();
    Task<Gateway?> GetById(long id);
    Task<List<GatewayCurrencyViewModel>> GetList();
    Task<GatewayCurrencyViewModel?> GetViewModelById(long id);
    Task<long> Insert(Gateway entity);
    Task<int> Update(Gateway entity);
}