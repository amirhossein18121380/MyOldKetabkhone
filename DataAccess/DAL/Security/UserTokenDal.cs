using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface;
using DataAccess.Interface.Security;
using DataAccess.Tool;
using DataModel.Models;
//using PokerNet.DataAccess.Dal.Contract;
//using PokerNet.DataAccess.Dal.Helper;
//using PokerNet.DataModel.Models.Security;

namespace DataAccess.DAL.Security;

public class UserTokenDal : IUserToken
{
    private readonly Entity? _context;

    #region DataMember
    private const string TbName = "[dbo].[UserToken]";
    #endregion

    #region Fetch
    public async Task<UserToken> GetById(long id)
    {
        var db = new DbEntityObject().GetConnectionString();
        //var db = _context.CreateConnection();
        try
        {
            var result = await db.QueryAsync<UserToken>($@"Select * From {TbName} WHERE Id = @id",
                new { id });

            return result.SingleOrDefault();
        }
        finally
        {
            db.Dispose();
        }
    }

    public async Task<UserToken> GetByAccessToken(string accessToken)
    {
        //var db = new DbEntityObject().GetConnectionString();
        var db = _context.CreateConnection();
        try
        {
            var result = await db.QueryAsync<UserToken>($@"Select * From {TbName} WHERE AccessTokenHash = @accessToken",
                new { accessToken });

            return result.SingleOrDefault();
        }
        finally
        {
            db.Dispose();
        }
    }

    public async Task<UserToken> GetByRefreshToken(string refreshToken)
    {
        var db = new DbEntityObject().GetConnectionString();
        //var db = _context.CreateConnection();
        try
        {
            var result = await db.QueryAsync<UserToken>($@"Select * From {TbName} WHERE RefreshTokenIdHash = @refreshToken",
                new { refreshToken });

            return result.SingleOrDefault();
        }
        finally
        {
            db.Dispose();
        }
    }

    public async Task<List<UserToken>> GetListByUserId(long userId)
    {
        var db = new DbEntityObject().GetConnectionString();
        //var db = _context.CreateConnection();
        try
        {
            var result = await db.QueryAsync<UserToken>($@"Select * From {TbName} WHERE UserId = @userId ", new { userId });

            return result.ToList();
        }
        finally
        {
            db.Dispose();
        }
    }

    public async Task<List<UserToken>> GetSameRefreshToken(string refreshTokenIdHash)
    {
        var db = new DbEntityObject().GetConnectionString();
        //var db = _context.CreateConnection();
        try
        {
            var result = await db.QueryAsync<UserToken>($@"Select * From {TbName} WHERE RefreshTokenIdHash = @refreshTokenIdHash
                                                                                               OR RefreshTokenIdHash = @refreshTokenIdHash 
                                                                                               AND RefreshTokenIdHash IS NULL", new { refreshTokenIdHash });

            return result.ToList();
        }
        finally
        {
            db.Dispose();
        }
    }

    public async Task<List<UserToken>> GetExpiredTokens()
    {
        var db = new DbEntityObject().GetConnectionString();
        //var db = _context.CreateConnection();
        try
        {
            var result = await db.QueryAsync<UserToken>($@"Select * From {TbName} WHERE RefreshTokenExpiresDateTime < GETDATE()");

            return result.ToList();
        }
        finally
        {
            db.Dispose();
        }
    }

    public async Task<UserToken> GetUserAccessToken(long userId, string accessToken)
    {
        var db = new DbEntityObject().GetConnectionString();
        //var db = _context.CreateConnection();

        try
        {
            var result = await db.QueryAsync<UserToken>($@"Select * From {TbName} WHERE UserId = @userId AND AccessTokenHash = @accessToken",
                new { userId, accessToken });

            return result.SingleOrDefault();
        }
        finally
        {
            db.Dispose();
        }
    }
    #endregion

    #region Insert
    public async Task<long> Insert(UserToken entity)
    {
        var db = new DbEntityObject().GetConnectionString();
        //var db = _context.CreateConnection();
        try
        {
            var prams = new DynamicParameters();
            prams.Add("@UserId", entity.UserId);
            prams.Add("@AccessTokenHash", entity.AccessTokenHash);
            prams.Add("@AccessTokenExpiresDateTime", entity.AccessTokenExpiresDateTime);
            prams.Add("@RefreshTokenIdHash", entity.RefreshTokenIdHash);
            prams.Add("@RefreshTokenExpiresDateTime", entity.RefreshTokenExpiresDateTime);
            prams.Add("@CreateOn", entity.CreateOn);

            var entityId = (await db.QueryAsync<long>(
                $@"INSERT INTO {TbName} 
                               (
                                       [UserId]
                                      ,[AccessTokenHash]
                                      ,[AccessTokenExpiresDateTime]
                                      ,[RefreshTokenIdHash]
                                      ,[RefreshTokenExpiresDateTime]
                                      ,[CreateOn]
                               )
                               VALUES
                               (
                                       @UserId
                                      ,@AccessTokenHash
                                      ,@AccessTokenExpiresDateTime
                                      ,@RefreshTokenIdHash
                                      ,@RefreshTokenExpiresDateTime
                                      ,@CreateOn
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)", prams)).SingleOrDefault();

            return entityId;
     
        }
        finally
        {
            //await db.DisposeAsync();
            db.Dispose();
        }
    }
    #endregion

    #region Update
    public async Task<int> Update(UserToken entity)
    {
        var db = new DbEntityObject().GetConnectionString();
        //var db = _context.CreateConnection();
        try
        {
            var sqlQuery = $@"UPDATE {TbName} 
                                   SET 
                                        [UserId] = @UserId
                                       ,[AccessTokenHash] = @AccessTokenHash
                                       ,[AccessTokenExpiresDateTime] = @AccessTokenExpiresDateTime
                                       ,[RefreshTokenIdHash] = @RefreshTokenIdHash
                                       ,[RefreshTokenExpiresDateTime] = @RefreshTokenExpiresDateTime
                                       ,[CreateOn] = @CreateOn
                                   WHERE Id = @Id";

            var rowsAffected = await db.ExecuteAsync(sqlQuery, new
            {
                entity.UserId,
                entity.AccessTokenHash,
                entity.AccessTokenExpiresDateTime,
                entity.RefreshTokenIdHash,
                entity.RefreshTokenExpiresDateTime,
                entity.CreateOn,
                entity.Id
            });

            return rowsAffected;
        }
        finally
        {
            //await db.DisposeAsync();
            db.Dispose();
        }
    }
    #endregion

    #region Delete
    public async Task<bool> Delete(long id)
    {
        var db = new DbEntityObject().GetConnectionString();
        //var db = _context.CreateConnection();
        try
        {
            var sqlQuery = $@"DELETE FROM {TbName} WHERE Id = @Id";
            var rowsCount = await db.ExecuteAsync(sqlQuery, new { id });
            return rowsCount > 0;
        }
        finally
        {
            //await db.DisposeAsync();
            db.Dispose();
        }
    }
    #endregion

}
