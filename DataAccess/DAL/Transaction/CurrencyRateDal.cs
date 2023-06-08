
using Dapper;
using DataAccess.Interface.Transaction;
using DataAccess.Tool;
using DataModel.Models.Transaction;
using DataModel.ViewModel.Transaction;

namespace DataAccess.DAL.Transaction;

public class CurrencyRateDal : ICurrencyRateDal
{
    #region DataMember
    private const string TableName = @"Transaction.CurrencyRate";
    #endregion

    #region Fetch
    public async Task<List<CurrencyRate>> GetList()
    {
        using var db = new DbEntityObject().GetConnectionString();

        return (await db.QueryAsync<CurrencyRate>($@"Select * From {TableName}")).ToList();
    }

    public async Task<List<CurrencyRateViewModel>> GetActiveList()
    {
        using var db = new DbEntityObject().GetConnectionString();

        return (await db.QueryAsync<CurrencyRateViewModel>($@"SELECT 
                                                                           Cur.Id
                                                                          ,Cur.SourceCurrencyId
                                                                          ,Cur.DestinationCurrencyId
                                                                          ,Cur.RateValue
                                                                          ,Cur.IsActive
                                                                          ,Cur.CreatorId
                                                                          ,Cur.CreateOn
	                                                                      ,sCu.Title AS SourceCurrencyTitle
	                                                                      ,dCu.Title AS DestinationCurrencyTitle
                                                                      FROM {TableName} AS Cur
                                                                      INNER JOIN Transaction.Currency AS sCu ON Cur.SourceCurrencyId = sCu.Id
                                                                      INNER JOIN Transaction.Currency AS dCu ON Cur.DestinationCurrencyId = dCu.Id
                                                                      WHERE Cur.IsActive = true")).ToList();
    }

    public async Task<List<CurrencyRate>> GetListByDestinationCurrencyId(int destinationCurrencyId)
    {
        using var db = new DbEntityObject().GetConnectionString();

        return (await db.QueryAsync<CurrencyRate>($@"Select * From {TableName}
                                                                 WHERE DestinationCurrencyId = @destinationCurrencyId",
            new { destinationCurrencyId })).ToList();

    }

    public async Task<CurrencyRate?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<CurrencyRate>($@"Select * From {TableName} WHERE Id = @id",
            new { id });

        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(CurrencyRate entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@SourceCurrencyId", entity.SourceCurrencyId);
        prams.Add("@DestinationCurrencyId", entity.DestinationCurrencyId);
        prams.Add("@RateValue", entity.RateValue);
        prams.Add("@IsActive", entity.IsActive);
        prams.Add("@CreatorId", entity.CreatorId);
        prams.Add("@CreateOn", entity.CreateOn);

        var entityId = (await db.QueryAsync<long>(
            $@"INSERT INTO {TableName} 
                               (
                                       SourceCurrencyId
                                      ,DestinationCurrencyId
                                      ,RateValue
                                      ,IsActive
                                      ,CreatorId
                                      ,CreateOn
                               )
                               VALUES
                               (
                                       @SourceCurrencyId
                                      ,@DestinationCurrencyId
                                      ,@RateValue
                                      ,@IsActive
                                      ,@CreatorId
                                      ,@CreateOn
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT);", prams)).SingleOrDefault();

        return entityId;
    }
    #endregion

    #region Update
    public async Task<int> Update(CurrencyRate entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        SourceCurrencyId = @SourceCurrencyId
                                       ,DestinationCurrencyId = @DestinationCurrencyId
                                       ,RateValue = @RateValue
                                       ,IsActive = @IsActive
                                       ,CreatorId = @CreatorId
                                       ,CreateOn = @CreateOn
                                   WHERE Id = @Id";

        var rowsAffected = await db.ExecuteAsync(sqlQuery, new
        {
            entity.SourceCurrencyId,
            entity.DestinationCurrencyId,
            entity.RateValue,
            entity.IsActive,
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

        var sqlQuery = $@"DELETE FROM {TableName} WHERE ""Id"" = @Id";
        var rowsCount = await db.ExecuteAsync(sqlQuery, new { id });
        return rowsCount > 0;
    }
    #endregion
}
