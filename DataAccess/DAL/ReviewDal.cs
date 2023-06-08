using Dapper;
using DataAccess.Interface;
using DataAccess.Tool;
using DataModel.Models;
using DataModel.ViewModel;

namespace DataAccess.DAL;

public class ReviewDal : IReviewDal
{
    #region DataMember
    private const string TbName = "[dbo].[Review]";
    #endregion

    #region Fetch
    public async Task<(List<Review> data, int totalCount)> GetList(ReviewGetListViewModel filterModel)
    {
        using var db = new DbEntityObject().GetConnectionString();

        #region Set Where Param
        var prams = new DynamicParameters();

        var whereQuery = string.Empty;

        if (filterModel.UserId.HasValue)
        {
            whereQuery += @"AND us.UserId = @UserId ";
            prams.Add("UserId", filterModel.UserId.Value);
        }
        else
        {
            if (filterModel.ParentId > 0)
            {
                whereQuery += @"AND us.ParentId LIKE @ParentId ";
                prams.Add("ParentId", $"%{filterModel.ParentId}%");
            }

            if (filterModel.EntityId > 0)
            {
                whereQuery += @"AND us.EntityId LIKE @EntityId ";
                prams.Add("EntityId", $"%{filterModel.EntityId}%");
            }

            if (filterModel.CommentDate.HasValue)
            {
                whereQuery += @"AND DATE_TRUNC('day', us.CommentDate) = @CommentDate ";
                prams.Add("CommentDate", filterModel.CommentDate.Value);
            }
        }
        #endregion

        #region Sql Query
        var skip = 0;
        if (filterModel.PageNumber > 0)
        {
            skip = filterModel.PageNumber * filterModel.PageSize;
        }

        prams.Add("Skip", skip);

        whereQuery = whereQuery.StartsWith("AND") ? $"WHERE{whereQuery.Substring(3, whereQuery.Length - 3)}" : whereQuery;

        var sqlQuery = $@"SELECT 
                                       us.Id
                                      ,us.ParentId
                                      ,us.UserId
                                      ,us.EntityType
                                      ,us.EntityId
                                      ,us.CommentValue
                                      ,us.CommentDate
                                  FROM {TbName} AS us
                                  {whereQuery}
                                  ORDER BY us.Id DESC OFFSET @Skip ROWS FETCH NEXT {filterModel.PageSize} ROWS ONLY;


                                  Select COUNT(1) 
                                  FROM {TbName}
                                  {whereQuery}";

        #endregion

        #region Get Data From Db
        using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);

        var data = (await multiData.ReadAsync<Review>()).ToList();
        var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

        return (data, totalCount);

        #endregion
    }

    public async Task<(List<Review> data, int totalCount)> GetParentList(ReviewGetListViewModel filterModel)
    {
        using var db = new DbEntityObject().GetConnectionString();

        #region Set Where Param
        var prams = new DynamicParameters();

        var whereQuery = $@"where ParentId is null and EntityType = {filterModel.EntityType} ";

        if (filterModel.UserId.HasValue)
        {
            whereQuery += @"AND us.UserId = @UserId ";
            prams.Add("UserId", filterModel.UserId.Value);
        }
        else
        {
            //if (filterModel.ParentId > 0)
            //{
            //    whereQuery += @"AND us.ParentId LIKE @ParentId ";
            //    prams.Add("ParentId", $"%{filterModel.ParentId}%");
            //}

            if (filterModel.EntityId > 0)
            {
                whereQuery += @"AND us.EntityId LIKE @EntityId ";
                prams.Add("EntityId", $"%{filterModel.EntityId}%");
            }

            if (filterModel.CommentDate.HasValue)
            {
                whereQuery += @"AND DATE_TRUNC('day', us.CommentDate) = @CommentDate ";
                prams.Add("CommentDate", filterModel.CommentDate.Value);
            }
        }
        #endregion

        #region Sql Query
        var skip = 0;
        if (filterModel.PageNumber > 0)
        {
            skip = filterModel.PageNumber * filterModel.PageSize;
        }

        prams.Add("Skip", skip);

        //whereQuery = whereQuery.StartsWith("AND") ? $"WHERE{whereQuery.Substring(3, whereQuery.Length - 3)}" : whereQuery;

        var sqlQuery = $@"SELECT 
                                       us.Id
                                      ,us.ParentId
                                      ,us.UserId
                                      ,us.EntityType
                                      ,us.EntityId
                                      ,us.CommentValue
                                      ,us.CommentDate
                                  FROM {TbName} AS us
                                  {whereQuery}
                                  ORDER BY us.Id DESC OFFSET @Skip ROWS FETCH NEXT {filterModel.PageSize} ROWS ONLY;


                                  Select COUNT(1) 
                                  FROM {TbName}
                                  {whereQuery}";
        #endregion
  
        #region Get Data From Db

        using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);

        var data = (await multiData.ReadAsync<Review>()).ToList();
        var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

        return (data, totalCount);

        #endregion
    }

    public async Task<(List<Review> data, int totalCount)> GetChildrenList(ReviewGetListViewModel filterModel)
    {
        using var db = new DbEntityObject().GetConnectionString();

        #region Set Where Param
        var prams = new DynamicParameters();

        var whereQuery = $@"where EntityType = {filterModel.EntityType} and ParentId = {filterModel.EntityId} and EntityId = {filterModel.EntityId} ";

        if (filterModel.UserId.HasValue)
        {
            whereQuery += @"AND us.UserId = @UserId ";
            prams.Add("UserId", filterModel.UserId.Value);
        }
        else
        {
            if (filterModel.ParentId > 0)
            {
                whereQuery += @"AND us.ParentId LIKE @ParentId ";
                prams.Add("ParentId", $"%{filterModel.ParentId}%");
            }
            if (filterModel.EntityId > 0)
            {
                whereQuery += @"AND us.EntityId LIKE @EntityId ";
                prams.Add("EntityId", $"%{filterModel.EntityId}%");
            }

            if (filterModel.CommentDate.HasValue)
            {
                whereQuery += @"AND DATE_TRUNC('day', us.CommentDate) = @CommentDate ";
                prams.Add("CommentDate", filterModel.CommentDate.Value);
            }
        }
        #endregion

        #region Sql Query
        var skip = 0;
        if (filterModel.PageNumber > 0)
        {
            skip = filterModel.PageNumber * filterModel.PageSize;
        }

        prams.Add("Skip", skip);

        whereQuery = whereQuery.StartsWith("AND") ? $"WHERE{whereQuery.Substring(3, whereQuery.Length - 3)}" : whereQuery;

        var sqlQuery = $@"SELECT 
                                       us.Id
                                      ,us.ParentId
                                      ,us.UserId
                                      ,us.EntityType
                                      ,us.EntityId
                                      ,us.CommentValue
                                      ,us.CommentDate
                                  FROM {TbName} AS us
                                  {whereQuery}
                                  ORDER BY us.Id DESC OFFSET @Skip ROWS FETCH NEXT {filterModel.PageSize} ROWS ONLY;


                                  Select COUNT(1) 
                                  FROM {TbName}
                                  {whereQuery}";
        #endregion

        #region Get Data From Db

        using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);

        var data = (await multiData.ReadAsync<Review>()).ToList();
        var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

        return (data, totalCount);

        #endregion
    }

    public async Task<List<Review>> GetParents(short entitytype)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"select * from dbo.Review where ParentId is null and EntityType = @entitytype";
        var result = await db.QueryAsync<Review>(query, new { entitytype });
        return result.ToList();
    }

    public async Task<List<Review>> GetChildsByTypeAndId(short entitytype, long entityId)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"select * from dbo.Review where EntityType = @entitytype and ParentId = @entityId and EntityId = @entityId";
        var result = await db.QueryAsync<Review>(query, new { entitytype, entityId });
        return result.ToList();
    }

    public async Task<Review?> GetComment(CommentViewModel coview)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"Select * From {TbName} WHERE  
                       UserId = {coview.UserId} and EntityType = {coview.EntityType} and EntityId = {coview.EntityId}";

        var result = (await db.QueryAsync<Review>(query)).SingleOrDefault();
        return result;
    }

    public async Task<Review?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Review>($@"Select * From {TbName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }

    #endregion

    #region Insert
    public async Task<long> Insert(Review review)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@ParentId", review.ParentId);
        prams.Add("@UserId", review.UserId);
        prams.Add("@EntityType", review.EntityType);
        prams.Add("@EntityId", review.EntityId);
        prams.Add("@CommentValue", review.CommentValue);
        prams.Add("@CommentDate", review.CommentDate);

        var query = $@"INSERT INTO {TbName} 
                               (
                                       [ParentId]
                                      ,[UserId]
                                      ,[EntityType]
                                      ,[EntityId]
                                      ,[CommentValue]
                                      ,[CommentDate]   
                               )
                               VALUES
                               (
                                       @ParentId
                                      ,@UserId
                                      ,@EntityType
                                      ,@EntityId
                                      ,@CommentValue
                                      ,@CommentDate
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var result = (await db.QueryAsync<long>(query, prams)).SingleOrDefault();
        return result;
    }
    #endregion

    #region Update
    public async Task<int> Update(Review review)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TbName} 
                                   SET 
                                        [ParentId] = @ParentId
                                       ,[UserId] = @UserId
                                       ,[EntityType] = @EntityType
                                       ,[EntityId] = @EntityId
                                       ,[CommentValue] = @CommentValue
                                       ,[CommentDate] = @CommentDate                                      
                                   WHERE Id = @Id";

        var result = await db.ExecuteAsync(query, new
        {
            review.ParentId,
            review.UserId,
            review.EntityType,
            review.EntityId,
            review.CommentValue,
            review.CommentDate,
            review.Id
        });

        return result;
    }
    #endregion

    #region Delete
    public async Task<bool> Delete(long Id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QuerySingleOrDefaultAsync($@"DELETE  from {TbName} where Id=@Id", new { Id });
        return result > 0;
    }

    public async Task<bool> DeleteBy(CommentViewModel dl)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"DELETE From {TbName} WHERE  
                       UserId = {dl.UserId} and EntityType = {dl.EntityType} and EntityId = {dl.EntityId}";

        var result = await db.ExecuteAsync(query);
        return result > 0;
    }
    #endregion
}
