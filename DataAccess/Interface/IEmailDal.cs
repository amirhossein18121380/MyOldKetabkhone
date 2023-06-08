using DataModel.Common;

namespace DataAccess.Interface;

public interface IEmailDal
{
    Task<bool> Delete(long id);
    Task<EmailTemplate?> GetById(long id);
    Task<List<EmailTemplate>> GetList();
    Task<long> Insert(EmailTemplate entity);
    Task<int> Update(EmailTemplate entity);
}