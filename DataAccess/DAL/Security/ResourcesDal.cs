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

public class ResourcesDal : IResourcesDal
{
    #region DataMember
    private const string TbName = "[dbo].[Resources]";
    #endregion

    #region Fetch
    public async Task<List<Resources>> GetList()
    {
        using var db = new DbEntityObject().GetConnectionString();
        return (await db.QueryAsync<Resources>($@"Select * From {TbName}")).ToList();
    }

    public async Task<Resources?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = (await db.QueryAsync<Resources>($@"Select * From {TbName} WHERE Id = @id", new { id })).SingleOrDefault();
        return result;
    }

    #endregion

    #region Insert
    public async Task<long> Insert(Resources entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@ResourceKey", entity.ResourceKey);
        prams.Add("@ResourceName", entity.ResourceName);
        prams.Add("@CreateOn", entity.CreateOn);

        var entityId = (await db.QueryAsync<long>(
            $@"INSERT INTO {TbName} 
                               (
                                       [ResourceKey]
                                      ,[ResourceName]
                                      ,[CreateOn]
                               )
                               VALUES
                               (
                                       @ResourceKey
                                      ,@ResourceName
                                      ,@CreateOn
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)", prams)).SingleOrDefault();

        return entityId;


    }
    #endregion

    #region Update
    public async Task<int> Update(Resources entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var sqlQuery = $@"UPDATE {TbName} 
                                   SET 
                                        [ResourceKey] = @ResourceKey
                                       ,[ResourceName] = @ResourceName
                                       ,[CreateOn] = @CreateOn                                       
                                   WHERE Id = @Id";

        var rowsAffected = await db.ExecuteAsync(sqlQuery, new
        {
            entity.ResourceKey,
            entity.ResourceName,
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

        var sqlQuery = $@"DELETE FROM {TbName} WHERE Id = @Id";
        var rowsCount = await db.ExecuteAsync(sqlQuery, new { id });
        return rowsCount > 0;

    }
    #endregion
}