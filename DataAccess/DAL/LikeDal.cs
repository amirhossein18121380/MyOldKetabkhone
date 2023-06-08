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

public class LikeDal : ILikeDal
{

    #region DataMember
    private const string TbName = "[dbo].[Like]";
    #endregion

    #region Fetch
    public async Task<Like?> GetLike(LikeViewModel dl)
    {
        using var db = new DbEntityObject().GetConnectionString();

        //var prams = new DynamicParameters();

        var query = $@"Select * From {TbName} WHERE  
                       UserId = {dl.UserId} and EntityType = {dl.EntityType} and EntityId = {dl.EntityId}";

        var result = await db.QueryAsync<Like>(query);
        return result.SingleOrDefault();
    }

    public async Task<List<int>> GetTotalLikeByEntityIdAndEntityType(short entitytype, long entityid)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();


        var query = $@"SELECT COUNT(*) as likes
                       FROM [dbo].[Like] as li 
                       where li.EntityType = {entitytype} and li.EntityId = {entityid} and li.Type = 1

                       SELECT COUNT(*) as DesLike
                       FROM [dbo].[Like] as li 
                        where li.EntityType = {entitytype} and li.EntityId = {entityid} and li.Type = 2";

        using var result = await db.QueryMultipleAsync(query);
        var data = (await result.ReadAsync<int>()).ToList();

        return data;
    }

    public async Task<List<Like>> GetAll()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Like>($@"SELECT * from {TbName}");
        return result.ToList();
    }

    public async Task<Like?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Like>($@"Select * From {TbName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(Like like)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@userId", like.UserId);
        prams.Add("@EntityType", like.EntityType);
        prams.Add("@EntityId", like.EntityId);
        prams.Add("@CreateOn", like.CreateOn);
        prams.Add("@Type", like.Type);

        var query = $@"INSERT INTO {TbName} 
                               (
                                       [userId]
                                      ,[EntityType]
                                      ,[EntityId]
                                      ,[CreateOn]
                                      ,[Type]
                               )
                               VALUES
                               (
                                       @userId
                                      ,@EntityType
                                      ,@EntityId
                                      ,@CreateOn
                                      ,@Type
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var result = await db.QuerySingleOrDefaultAsync<long>(query, prams);
        return result;
    }
    #endregion

    #region Update
    public async Task<int> Update(Like like)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TbName} 
                                   SET 
                                        [UserId] = @UserId
                                       ,[EntityType] = @EntityType
                                       ,[EntityId] = @EntityId
                                       ,[CreateOn] = @CreateOn
                                       ,[Type] = @Type
                                   WHERE Id = @Id";

        var result = await db.ExecuteAsync(query, new
        {
            like.UserId,
            like.EntityType,
            like.EntityId,
            like.CreateOn,
            like.Type,
            like.Id
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

    public async Task<bool> DeleteBy(LikeViewModel dl)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"DELETE From {TbName} WHERE  
                       UserId = {dl.UserId} and EntityType = {dl.EntityType} and EntityId = {dl.EntityId}";

        var result = await db.ExecuteAsync(query);
        return result > 0;
    }
    #endregion
}