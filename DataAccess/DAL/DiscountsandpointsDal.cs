using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Tool;
using DataModel.Models;

namespace DataAccess.DAL;

public class DiscountsandpointsDal
{
    #region DataMember
    private const string TbName = "[dbo].[Discountsandpoints]";
    #endregion

    #region Fetch

    public async Task<List<Discountsandpoints>> GetAll()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Discountsandpoints>($@"SELECT * from {TbName}");
        return result.ToList();
    }

    public async Task<Discountsandpoints?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Discountsandpoints>($@"Select * From {TbName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(Discountsandpoints discountsandpoints)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@DiscountCoupon", discountsandpoints.DiscountCoupon);
        prams.Add("@PercentAmount", discountsandpoints.PercentAmount);
        prams.Add("@IsCredit", discountsandpoints.IsCredit);
        prams.Add("@DiscountActivationTime", discountsandpoints.DiscountActivationTime);
        prams.Add("@DiscountExpirationTime", discountsandpoints.DiscountExpirationTime);
        prams.Add("@BookId", discountsandpoints.BookId);
        prams.Add("@UserId", discountsandpoints.UserId);

        var query = $@"INSERT INTO {TbName} 
                               (
                                       [DiscountCoupon]
                                      ,[PercentAmount]
                                      ,[IsCredit]
                                      ,[DiscountActivationTime]
                                      ,[DiscountExpirationTime]
                                      ,[BookId]
                                      ,[UserId]
                               )
                               VALUES
                               (
                                       @DiscountCoupon
                                      ,@PercentAmount
                                      ,@IsCredit
                                      ,@DiscountActivationTime
                                      ,@DiscountExpirationTime
                                      ,@BookId
                                      ,@UserId                               
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var result = await db.QuerySingleOrDefaultAsync<long>(query, prams);
        return result;
    }
    #endregion

    #region Update
    public async Task<int> Update(Discountsandpoints authorBook)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TbName} 
                                   SET 
                                        [DiscountCoupon] = @DiscountCoupon
                                       ,[PercentAmount] = @PercentAmount
                                       ,[IsCredit] = @IsCredit
                                       ,[DiscountActivationTime] = @DiscountActivationTime
                                       ,[DiscountExpirationTime] = @DiscountExpirationTime
                                       ,[BookId] = @AuthorId
                                       ,[UserId] = @BookId
                                   WHERE Id = @Id";

        var result = await db.ExecuteAsync(query, new
        {
            authorBook.DiscountCoupon,
            authorBook.PercentAmount,
            authorBook.IsCredit,
            authorBook.DiscountActivationTime,
            authorBook.DiscountExpirationTime,
            authorBook.BookId,
            authorBook.UserId,
            authorBook.Id
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
