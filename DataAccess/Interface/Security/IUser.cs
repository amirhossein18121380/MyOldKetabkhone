using Common.Enum;
using DataModel.Models;
using DataModel.ViewModel.Security.User;

namespace DataAccess.Interface.Security;

public interface IUser
{
    Task<(List<UserGetListViewModel> data, int totalCount)> GetList(UserGetListFilterViewModel filterModel);
    Task<User?> GetByEmail(string email);
    Task<List<string>> GetUserEmails(EmailSendTypeEnum sendType, string? filterValue);
    Task<User?> GetByEmailAndPassword(string email, string password);
    Task<User?> GetByForgetPasswordToken(string forgetPasswordToken);
    Task<User?> GetById(long id);
    Task<List<User>> GetByIds(long[] ids);
    Task<User?> GetByMobileNumber(string mobileNumber);
    Task<User?> GetByNationalCode(string nationalCode);
    Task<User?> GetByProfilePictureName(string fileName);
    Task<User?> GetByUniqCode(string uniqCode);
    Task<User?> GetByUserName(string userName);
    Task<User?> GetByUserNameAndPassword(string userName, string password);
    //Task<List<User>> GetList();
    Task<int> GetRegistrationIpCount(string registrationIp);
    Task<long> Insert(User entity);
    Task<int> Update(User entity);
}