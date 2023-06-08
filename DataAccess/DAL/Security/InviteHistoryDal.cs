
using Dapper;
using DataAccess.Interface.Security;
using DataAccess.Tool;
using DataModel.Models.Security;
using PokerNet.DataModel.ViewModel.Security;

namespace DataAccess.DAL.Security;

public class InviteHistoryDal : IInviteHistoryDal
{
    #region DataMember
    private const string TableName = "[Security].[InviteHistory]";
    #endregion

    #region Fetch
    public async Task<(List<InviteHistoryViewModel>? data, int totalCount)> GetListByFilter(InviteHistoryFilterViewModel filterModel)
    {
        using var db = new DbEntityObject().GetConnectionString();

        #region Set Where Param
        if (filterModel.ParentUserId <= 0)
        {
            return (null, 0);
        }

        var prams = new DynamicParameters();

        var whereQuery = @"WHERE ih.ParentUserId = @ParentUserId ";
        prams.Add("ParentUserId", filterModel.ParentUserId);

        if (filterModel.UserId.HasValue)
        {
            whereQuery += @"AND ih.UserId = @UserId ";
            prams.Add("UserId", filterModel.UserId.Value);
        }

        if (filterModel.IsGetGift.HasValue)
        {
            whereQuery += @"AND ih.IsGetGift = @IsGetGift ";
            prams.Add("IsGetGift", filterModel.IsGetGift.Value);
        }

        if (filterModel.EmailVerified.HasValue)
        {
            whereQuery += @"AND ih.EmailVerified = @EmailVerified ";
            prams.Add("EmailVerified", filterModel.EmailVerified.Value);
        }

        if (filterModel.FromRegisterDate.HasValue)
        {
            whereQuery += @"AND DATE_TRUNC('day', ih.RegisterDate) >= @FromRegisterDate ";
            prams.Add("FromRegisterDate", filterModel.FromRegisterDate.Value);
        }

        if (filterModel.ToRegisterDate.HasValue)
        {
            whereQuery += @"AND DATE_TRUNC('day', ih.RegisterDate) <= @ToRegisterDate ";
            prams.Add("ToRegisterDate", filterModel.ToRegisterDate.Value);
        }
        #endregion

        #region Sql Query
        var skip = 0;
        if (filterModel.PageNumber > 0)
        {
            skip = filterModel.PageNumber * filterModel.PageSize;
        }

        prams.Add("Skip", skip);

        var sqlQuery = $@"SELECT 
                                         ih.Id
                                        ,ih.ParentUserId
                                        ,ih.UserId
                                        ,ih.RegisterDate
                                        ,ih.IsGetGift
                                        ,ih.GiftAmount
                                        ,ih.GiftDate
                                        ,us.EmailVerified
                                        ,CASE WHEN EXISTS(SELECT UserId
                                  		                FROM Transaction.TransactionInfo
                                  		                WHERE UserId = ih.UserId AND 
                                  						      OperationType = 1 AND 
                                                              StatusType = 2 AND 
                                                              PaymentMethod NOT IN (7,8,11)) 
                                               THEN true
                                               ELSE false
                                         END AS HasSuccessDeposit
                                  FROM {TableName} AS ih
                                  INNER JOIN Security.User AS us ON ih.UserId = us.Id
                                  {whereQuery}
                                  ORDER BY ih.Id DESC OFFSET @Skip ROWS FETCH NEXT {filterModel.PageSize} ROWS ONLY;

                                  Select COUNT(1) 
                                  FROM {TableName} AS ih
                                  INNER JOIN Security.User AS us ON ih.UserId = us.Id
                                  {whereQuery}";


        #endregion

        #region Get Data From Db
        using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);

        var data = (await multiData.ReadAsync<InviteHistoryViewModel>()).ToList();
        var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

        return (data, totalCount);
        #endregion
    }

    public async Task<InviteHistory?> GetHistoryRecord(long userId)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<InviteHistory>($@"Select * From {TableName} 
                                                                                       WHERE UserId = @userId",
            new { userId });

        return result.SingleOrDefault();
    }

    public async Task<int> GetGiftCount(long parentUserId)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<int>($@"Select Count(1) From {TableName} 
                                                             WHERE ParentUserId = @parentUserId AND IsGetGift = true",
            new { parentUserId });

        return result.FirstOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(InviteHistory entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@ParentUserId", entity.ParentUserId);
        prams.Add("@UserId", entity.UserId);
        prams.Add("@RegisterDate", entity.RegisterDate);
        prams.Add("@IsGetGift", entity.IsGetGift);
        prams.Add("@GiftAmount", entity.GiftAmount);
        prams.Add("@GiftDate", entity.GiftDate);

        var entityId = (await db.QueryAsync<long>(
            $@"INSERT INTO {TableName} 
                               (
                                       ParentUserId
                                      ,UserId
                                      ,RegisterDate
                                      ,IsGetGift
                                      ,GiftAmount
                                      ,GiftDate
                               )
                               VALUES
                               (
                                       @ParentUserId
                                      ,@UserId
                                      ,@RegisterDate
                                      ,@IsGetGift
                                      ,@GiftAmount
                                      ,@GiftDate
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)", prams)).SingleOrDefault();

        return entityId;
    }
    #endregion

    #region Update
    public async Task<int> Update(InviteHistory entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        ParentUserId = @ParentUserId
                                       ,UserId = @UserId
                                       ,RegisterDate = @RegisterDate
                                       ,IsGetGift = @IsGetGift
                                       ,GiftAmount = @GiftAmount
                                       ,GiftDate = @GiftDate
                                   WHERE Id = @Id";

        var rowsAffected = await db.ExecuteAsync(sqlQuery, new
        {
            entity.ParentUserId,
            entity.UserId,
            entity.RegisterDate,
            entity.IsGetGift,
            entity.GiftAmount,
            entity.GiftDate,
            entity.Id
        });

        return rowsAffected;
    }
    #endregion

    #region Delete
    public async Task<bool> Delete(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var sqlQuery = $@"DELETE FROM {TableName} WHERE Id = @Id";
        var rowsCount = await db.ExecuteAsync(sqlQuery, new { id });
        return rowsCount > 0;
    }
    #endregion
}
