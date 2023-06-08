using DataModel.Models;

namespace DataAccess.Interface;

public interface ISubjectDal
{
    Task<bool> Delete(long Id);
    Task<List<Subjects>> GetAll();
    Task<Subjects?> GetById(long id);
    Task<long> Insert(Subjects sub);
    Task<int> Update(Subjects sub);
}