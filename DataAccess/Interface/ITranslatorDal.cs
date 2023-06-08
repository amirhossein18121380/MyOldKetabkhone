using DataModel.Models;
using DataModel.ViewModel;

namespace DataAccess.Interface;

public interface ITranslatorDal
{
    Task<bool> Delete(long Id);
    Task<List<Translator>> GetAll();
    Task<(List<TranslatorGetListViewModel> data, int totalCount)> GetList(TranslatorGetListFilterViewModel filterModel);
    //Task<Translator?> GetByEmail(string? email);
    Task<Translator?> GetByFirstnameAndLastname(string? translatorfirstname, string? translatorlastname);
    Task<Translator?> GetById(long id);
    Task<long> Insert(Translator translator);
    Task<int> Update(Translator translator);
}