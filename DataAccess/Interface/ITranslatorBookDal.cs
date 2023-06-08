using DataModel.Models;

namespace DataAccess.Interface;

public interface ITranslatorBookDal
{
    Task<bool> Delete(long Id);
    Task<List<TranslatorBook>> GetAll();
    Task<bool> DeleteByBookId(long bookId);
    Task<TranslatorBook?> GetById(long id);
    Task<long> Insert(TranslatorBook translatorBook);
    Task<long> Update(TranslatorBook translatorBook);
}