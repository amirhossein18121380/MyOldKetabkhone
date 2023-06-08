using DataModel.Models;

namespace DataAccess.Interface;

public interface ICategoryDal
{
    Task<bool> Delete(long Id);
    Task<List<Category>> GetAll();
    Task<Category?> GetById(long id);
    Task<List<Category>> GetChild(long parentid);
    Task<List<Category>> GetParent();
    Task<bool> Insert(Category category);
    Task<bool> AddChild(Category category);
    Task<int> Update(Category category);
}