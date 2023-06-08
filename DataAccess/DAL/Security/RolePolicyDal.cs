using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helper;
using Dapper;
using DataAccess.Interface.Security;
using DataAccess.Tool;
using DataModel.Models;

namespace DataAccess.DAL.Security;

public class RolePolicyDal : IRolePolicyDal
{
    #region DataMember
    private const string TbName = "[dbo].[RolePolicy]";
    #endregion

    #region Fetch
    public async Task<List<RolePolicy>> GetList()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = (await db.QueryAsync<RolePolicy>($@"Select * From {TbName}")).ToList();
        return result;
    }
    public async Task<RolePolicy?> GetList(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = (await db.QueryAsync<RolePolicy>($@"Select * From {TbName} WHERE Id = @id", new { id })).SingleOrDefault();
        return result;
    }
    public async Task<List<RolePolicy>> GetByRoleId(long roleId)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = (await db.QueryAsync<RolePolicy>($@"Select * From {TbName} WHERE RoleId = @roleId", new { roleId })).ToList();
        return result;
    }
    public async Task<long[]> GetResourceIdsByUserId(long userId)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<long>($@"SELECT DISTINCT RP.ResourceId
                                                             FROM [dbo].[RoleMember] AS RM
                                                             INNER JOIN [dbo].[RolePolicy] AS RP ON RM.RoleId = RP.RoleId
                                                             WHERE RM.UserId = @userId", new { userId });
        return result.ToArray();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(RolePolicy entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@RoleId", entity.RoleId);
        prams.Add("@ResourceId", entity.ResourceId);
        prams.Add("@CreatorId", entity.CreatorId);
        prams.Add("@CreateOn", entity.CreateOn);

        var entityId = (await db.QueryAsync<long>(GetInsertQuery(), prams)).SingleOrDefault();

        return entityId;
    }

    public string GetInsertQuery()
    {
        var query = $@"INSERT INTO {TbName} 
                               (
                                       [RoleId]
                                      ,[ResourceId]
                                      ,[CreatorId]
                                      ,[CreateOn]
                               )
                               VALUES
                               (
                                       @RoleId
                                      ,@ResourceId
                                      ,@CreatorId
                                      ,@CreateOn
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)";
        return query;
    }
    #endregion

    #region Update
    public async Task<int> Update(RolePolicy entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var sqlQuery = $@"UPDATE {TbName} 
                                   SET 
                                        [RoleId] = @RoleId
                                       ,[ResourceId] = @ResourceId
                                       ,[CreatorId] = @CreatorId
                                       ,[CreateOn] = @CreateOn
                                   WHERE Id = @Id";

        var rowsAffected = await db.ExecuteAsync(sqlQuery, new
        {
            entity.RoleId,
            entity.ResourceId,
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
        var query = $@"DELETE FROM {TbName} WHERE Id = @Id";
        var rowsCount = await db.ExecuteAsync(query, new { id });
        return rowsCount > 0;
    }
    #endregion

    #region SetRolePolicy
    public async Task<bool> SetRolePolicy(long roleId, long[] recourseIds, long creatorId)
    {
        await using var connection = new DbEntityObject().GetConnectionString();
        await connection.OpenAsync();
        await using var tran = connection.BeginTransaction();

        try
        {
            #region Delete Curent Policy

            var deleted = await connection.ExecuteAsync($@"DELETE FROM {TbName} WHERE RoleId = @roleId", new { roleId }, tran);

            if (deleted < 0)
            {
                tran.Rollback();
                return false;
            }
            #endregion

            #region Insert New Role Policy

            foreach (var recourseId in recourseIds)
            {
                var rowAff = (await connection.QueryAsync<long>(GetInsertQuery(),
                    new
                    {
                        RoleId = roleId,
                        CreatorId = creatorId,
                        CreateOn = DateTime.Now,
                        ResourceId = recourseId
                    }, tran)).FirstOrDefault();

                if (rowAff <= 0)
                {
                    tran.Rollback();
                    return false;
                }
            }
            #endregion

            tran.Commit();
            return true;
        }
        catch (Exception ex)
        {
            tran.Rollback();
            // LogHelper.ErrorLog(ex.Message, ex);
            LogHelper.ErrorLog("RolePolicyDal|SetRolePolicy", ex);
            return false;
        }
    }
    #endregion
}