using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface;
using DataAccess.Tool;
using DataModel.Models;
using DataModel.ViewModel;

namespace DataAccess.DAL;

public class RateDal : IRateDal
{
    #region DataMember
    private const string TbName = "[dbo].[Rate]";
    #endregion

    #region Fetch
    public async Task<Rate?> GetRate(GetRateViewModel getrate)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();

        //var whereQuery = string.Empty;

        //if (getrate.UserId.HasValue)
        //{
        //    whereQuery += @"AND us.Id = @Id ";
        //    prams.Add("Id", getrate.UserId.Value);
        //}
        //if (getrate.EntityType.HasValue)
        //{
        //    whereQuery += @"AND us.Id = @Id ";
        //    prams.Add("Id", getrate.EntityType.Value);
        //}
        //if (getrate.EntityId.HasValue)
        //{
        //    whereQuery += @"AND us.Id = @Id ";
        //    prams.Add("Id", getrate.EntityId.Value);
        //}

        //whereQuery = whereQuery.StartsWith("AND") ? $"WHERE{whereQuery.Substring(3, whereQuery.Length - 3)}" : whereQuery;

        var query = $@"Select * From {TbName} WHERE  
                       UserId = {getrate.UserId} and EntityType = {getrate.EntityType} and EntityId = {getrate.EntityId}";

        var result = await db.QueryAsync<Rate>(query, prams);
        return result.SingleOrDefault();
    }

    public async Task<List<Rate>> GetAll(int entitytype)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Rate>($@"SELECT * from {TbName} where EntityType = @entitytype");
        return result.ToList();
    }

    public async Task<Rate?> GetTheAvrRateByEntityIdAndEntityType(int entitytype, long entityid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"(SELECT CAST(AVG(CAST(RateValue AS  DECIMAL(10,1))) AS DECIMAL(10,1)) AS Rate FROM {TbName} where EntityType = @entitytype and EntityId = @entityid)";
        var result = await db.QueryAsync<Rate>(query, new { entitytype, entityid });
        return result.SingleOrDefault();
    }

    public async Task<Rate?> GetTheAvrRateByEntityIdForAuthors(long authorid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"(SELECT CAST(AVG(CAST(RateValue AS  DECIMAL(10,1))) AS DECIMAL(10,1)) AS Rate FROM {TbName} where EntityType = 1 and EntityId = @authorid)";
        var result = await db.QueryAsync<Rate>(query, new { authorid });
        return result.SingleOrDefault();
    }

    public async Task<Rate?> GetTheAvrRateByEntityIdForTranslators(long translatorid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"(SELECT CAST(AVG(CAST(RateValue AS  DECIMAL(10,1))) AS DECIMAL(10,1)) AS Rate FROM {TbName} where EntityType = 2 and EntityId = @translatorid)";
        var result = await db.QueryAsync<Rate>(query, new { translatorid });
        return result.SingleOrDefault();
    }

    public async Task<Rate?> GetTheAvrRateByEntityIdForUser(long userid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"(SELECT CAST(AVG(CAST(RateValue AS  DECIMAL(10,1))) AS DECIMAL(10,1)) AS Rate FROM {TbName} where EntityType = 3 and EntityId = @userid)";
        var result = await db.QueryAsync<Rate>(query, new { userid });
        return result.SingleOrDefault();
    }

    public async Task<decimal?> GetTheAvrRateByEntityIdForBook(long bookid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"(SELECT CAST(AVG(CAST(RateValue AS  DECIMAL(10,1))) AS DECIMAL(10,1)) AS Rate FROM {TbName} where EntityType = 4 and EntityId = @bookid)";
        var result = await db.QueryAsync<decimal>(query, new { bookid });
        return result.SingleOrDefault();
    }

    public async Task<Rate?> GetRateByEntityId(long entityid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Rate>($@"Select * From {TbName} WHERE EntityId = @entityid", new { entityid });
        return result.SingleOrDefault();
    }

    public async Task<Rate?> GetRateByUserId(long userid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Rate>($@"Select * From {TbName} WHERE UserId = @userid", new { userid });
        return result.SingleOrDefault();
    }

    public async Task<Rate?> GetRateByEntityTypeandEntityId(short entitytype, long entityid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"Select * From {TbName} WHERE  EntityType = @entitytype and EntityId = @entityid";
        var result = await db.QueryAsync<Rate>(query, new { entitytype, entityid });
        return result.SingleOrDefault();
    }

    public async Task<Rate?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Rate>($@"Select * From {TbName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(Rate rate)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@UserId", rate.UserId);
        prams.Add("@EntityType", rate.EntityType);
        prams.Add("@EntityId", rate.EntityId);
        prams.Add("@RateValue", rate.RateValue);
        prams.Add("@CreateOn", rate.CreateOn);

        var query = $@"INSERT INTO {TbName} 
                               (
                                       [UserId]
                                      ,[EntityType]
                                      ,[EntityId]
                                      ,[RateValue]
                                      ,[CreateOn]
                               )
                               VALUES
                               (
                                       @UserId
                                      ,@EntityType
                                      ,@EntityId
                                      ,@RateValue
                                      ,@CreateOn
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var result = (await db.QueryAsync<long>(query, prams)).SingleOrDefault();
        return result;
    }
    #endregion

    #region Update
    public async Task<int> Update(Rate rate)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TbName} 
                                   SET 
                                        [UserId] = @UserId
                                       ,[EntityType] = @EntityType
                                       ,[EntityId] = @EntityId
                                       ,[RateValue] = @RateValue
                                       ,[CreateOn] = @CreateOn
                                   WHERE Id = @Id";

        var result = await db.ExecuteAsync(query, new
        {
            rate.UserId,
            rate.EntityType,
            rate.EntityId,
            rate.RateValue,
            rate.CreateOn,
            rate.Id
        });

        return result;
    }
    #endregion

    #region Delete
    public async Task<bool> Delete(long Id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QuerySingleOrDefaultAsync($@"DELETE * from {TbName} where Id=@Id", new { Id });
        return result > 0;
    }
    #endregion
}
