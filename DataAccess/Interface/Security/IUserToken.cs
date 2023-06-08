using DataModel.Models;

namespace DataAccess.Interface.Security;

public interface IUserToken
{
    Task<bool> Delete(long id);
    Task<UserToken> GetByAccessToken(string accessToken);
    Task<UserToken> GetById(long id);
    Task<UserToken> GetByRefreshToken(string refreshToken);
    Task<List<UserToken>> GetExpiredTokens();
    Task<List<UserToken>> GetListByUserId(long userId);
    Task<List<UserToken>> GetSameRefreshToken(string refreshTokenIdHash);
    Task<UserToken> GetUserAccessToken(long userId, string accessToken);
    Task<long> Insert(UserToken entity);
    Task<int> Update(UserToken entity);
}