using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.Security;
using DataAccess.Tool;
using DataModel.Models;

namespace DataAccess.DAL.Security;

public class RoleMemberDal : IRoleMemberDal
{
    #region
    private const string TableName = "[dbo].[RoleMember]";
    #endregion

    #region Fetch
    public async Task<List<RoleMember>> GetList()
    {
        var db = new DbEntityObject().GetConnectionString();
        var result = (await db.QueryAsync<RoleMember>($@"Select * From {TableName}")).ToList();
        return result;
    }

    public async Task<RoleMember?> GetById(long id)
    {
        var db = new DbEntityObject().GetConnectionString();
        var result = (await db.QueryAsync<RoleMember>($@"Select * From {TableName} WHERE Id = @id", new { id })).SingleOrDefault();
        return result;
    }

    public async Task<List<RoleMember>> GetByUserId(long userId)
    {
        var db = new DbEntityObject().GetConnectionString();
        var result = (await db.QueryAsync<RoleMember>($@"Select * From {TableName} WHERE UserId = @userId", new { userId })).ToList();
        return result;
    }

    #endregion

    #region Insert
    public async Task<long> Insert(RoleMember roleMember)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var parameters = new DynamicParameters();
        parameters.Add("@RoleId", roleMember.RoleId);
        parameters.Add("@UserId", roleMember.UserId);
        parameters.Add("@CreatorId", roleMember.CreatorId);
        parameters.Add("@CreateOn", roleMember.CreateOn);

        var entityId = (await db.QueryAsync<long>(
            $@"INSERT INTO {TableName} 
                               (
                                       [RoleId]
                                      ,[UserId]
                                      ,[CreatorId]
                                      ,[CreateOn]
                               )
                               VALUES
                               (
                                       @RoleId
                                      ,@UserId
                                      ,@CreatorId
                                      ,@CreateOn
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)", parameters)).SingleOrDefault();

        return entityId;

    }
    #endregion

    #region Update
    public async Task<int> Update(RoleMember entity)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"UPDATE {TableName} 
                                   SET 
                                        [RoleId] = @RoleId
                                       ,[UserId] = @UserId
                                       ,[CreatorId] = @CreatorId
                                       ,[CreateOn] = @CreateOn                                       
                                   WHERE Id = @Id";

        var rowsAffected = await db.ExecuteAsync(query, new
        {
            entity.RoleId,
            entity.UserId,
            entity.CreatorId,
            entity.CreateOn,
            entity.Id
        });

        return rowsAffected;
    }
    #endregion

    #region Delete
    public async Task<bool> Delete(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"DELETE FROM {TableName} WHERE Id = @Id";
        var rowsCount = await db.ExecuteAsync(query, new { id });
        return rowsCount > 0;
    }

    public async Task<bool> DeleteUserRole(long userId)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"DELETE FROM {TableName} WHERE UserId = @userId";
        var rowsCount = await db.ExecuteAsync(query, new { userId });
        return rowsCount > 0;
    }
    #endregion
}
