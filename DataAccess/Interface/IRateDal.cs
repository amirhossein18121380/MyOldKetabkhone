using DataModel.Models;
using DataModel.ViewModel;

namespace DataAccess.Interface;

public interface IRateDal
{
    Task<bool> Delete(long Id);
    Task<List<Rate>> GetAll(int entitytype);
    Task<Rate?> GetById(long id);
    Task<Rate?> GetTheAvrRateByEntityIdAndEntityType(int entitytype, long entityid);
    Task<Rate?> GetTheAvrRateByEntityIdForTranslators(long translatorid);
    Task<Rate?> GetTheAvrRateByEntityIdForAuthors(long authorid);
    Task<Rate?> GetTheAvrRateByEntityIdForUser(long userid);
    Task<decimal?> GetTheAvrRateByEntityIdForBook(long bookid);
    Task<Rate?> GetRateByEntityId(long entityid);
    Task<Rate?> GetRateByUserId(long userid);
    Task<Rate?> GetRateByEntityTypeandEntityId(short entitytype, long entityid);
    Task<Rate?> GetRate(GetRateViewModel getrate);
    Task<long> Insert(Rate rate);
    Task<int> Update(Rate rate);
}