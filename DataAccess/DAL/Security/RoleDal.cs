#region usings
using Common.Helper;
using Dapper;
using DataAccess.Interface.Security;
using DataAccess.Tool;
using DataModel.Models;
#endregion

namespace DataAccess.DAL.Security;

public class RoleDal : IRoleDal
{
    #region DataConnection
    private const string TableName = "[dbo].[Role]";
    #endregion

    #region Fetch
    public async Task<List<Role>> GetList()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = (await db.QueryAsync<Role>($@"Select * from {TableName}")).ToList();
        return result;
    }

    public async Task<Role?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = (await db.QueryAsync<Role>($@"Select * from {TableName} where Id=@Id", new { id })).SingleOrDefault();
        return result;
    }

    #endregion

    #region Insert
    public async Task<long> Insert(Role role)
    {
        var db = new DbEntityObject().GetConnectionString();

        var parameters = new DynamicParameters();
        parameters.Add("@Title", role.Title);
        parameters.Add("@Comment", role.Comment);
        parameters.Add("@CreatorId", role.CreatorId);
        parameters.Add("@CreateOn", role.CreateOn);

        var entityId = (await db.QueryAsync<long>(
            $@"INSERT INTO {TableName} 
                               (
                                       [Title]
                                      ,[Comment]
                                      ,[CreatorId]
                                      ,[CreateOn]
                               )
                               VALUES
                               (
                                       @Title
                                      ,@Comment
                                      ,@CreatorId
                                      ,@CreateOn
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)", parameters)).SingleOrDefault();

        return entityId;

    }
    #endregion

    #region Update
    public async Task<int> Update(Role role)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TableName} 
                                   SET 
                                        [Title] = @Title
                                       ,[Comment] = @Comment
                                       ,[CreatorId] = @CreatorId
                                       ,[CreateOn] = @CreateOn                                       
                                   WHERE Id = @Id";

        var rowsAffected = (await db.QueryAsync<int>(query, new
        {
            role.Title,
            role.Comment,
            role.CreatorId,
            role.CreateOn,
            role.Id
        })).SingleOrDefault();

        return rowsAffected;
    }
    #endregion

    #region Delete
    public async Task<bool> Delete(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = (await db.QueryAsync<int>($@"Delete from {TableName} where Id=@Id", new { id })).SingleOrDefault();
        if (result! > 0)
        {
            LogHelper.ErrorLog("Something Wrong with RoleDal Or Can Not Delete this Role");
            return false;
        }
        return true;

        //return (result > 0) ? true : false;
    }
    #endregion
}