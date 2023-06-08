using DataModel.Models;
using DataModel.ViewModel;

namespace DataAccess.Interface;

public interface IReviewDal
{
    Task<bool> Delete(long Id);
    Task<bool> DeleteBy(CommentViewModel dl);
    Task<Review?> GetById(long id);
    Task<List<Review>> GetChildsByTypeAndId(short entitytype, long entityId);
    Task<Review?> GetComment(CommentViewModel coview);
    Task<List<Review>> GetParents(short entitytype);
    Task<long> Insert(Review review);
    Task<int> Update(Review review);
    Task<(List<Review> data, int totalCount)> GetList(ReviewGetListViewModel filterModel);
    Task<(List<Review> data, int totalCount)> GetParentList(ReviewGetListViewModel filterModel);
    Task<(List<Review> data, int totalCount)> GetChildrenList(ReviewGetListViewModel filterModel);
}