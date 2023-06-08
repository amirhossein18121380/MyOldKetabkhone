using DataModel.Models;
using DataModel.ViewModel;

namespace DataAccess.Interface;

public interface ILikeDal
{
    Task<bool> Delete(long Id);
    Task<List<Like>> GetAll();
    Task<Like?> GetById(long id);
    Task<Like?> GetLike(LikeViewModel dl);
    Task<List<int>> GetTotalLikeByEntityIdAndEntityType(short entitytype, long entityid);
    Task<long> Insert(Like like);
    Task<int> Update(Like like);
    Task<bool> DeleteBy(LikeViewModel dl);
}