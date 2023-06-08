
using Dapper;
using DataAccess.Interface.Transaction;
using DataAccess.Tool;
using DataModel.Models.Transaction;

namespace DataAccess.DAL.Transaction;

public class CurrencyDal : ICurrencyDal
{
    #region DataMember
    private const string TableName = @"[Transaction].[Currency]";
    #endregion

    #region Fetch
    public async Task<List<Currency>> GetList()
    {
        using var db = new DbEntityObject().GetConnectionString();

        return (await db.QueryAsync<Currency>($@"Select * From {TableName}")).ToList();
    }
    public async Task<Currency?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<Currency>($@"Select * From {TableName} WHERE Id = @id",
            new { id });

        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(Currency entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@Title", entity.Title);
        prams.Add("@Symbol", entity.Symbol);
        prams.Add("@Label", entity.Label);
        prams.Add("@CreatorId", entity.CreatorId);
        prams.Add("@CreateOn", entity.CreateOn);

        var entityId = (await db.QueryAsync<long>(
            $@"INSERT INTO {TableName} 
                               (
                                       Title
                                      ,Symbol
                                      ,Label
                                      ,CreatorId
                                      ,CreateOn
                               )
                               VALUES
                               (
                                       @Title
                                      ,@Symbol
                                      ,@Label
                                      ,@CreatorId
                                      ,@CreateOn
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT);", prams)).SingleOrDefault();

        return entityId;
    }
    #endregion

    #region Update
    public async Task<int> Update(Currency entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        Title = @Title
                                       ,Symbol = @Symbol
                                       ,Label = @Label
                                       ,CreatorId = @CreatorId
                                       ,CreateOn = @CreateOn
                                   WHERE Id = @Id";

        var rowsAffected = await db.ExecuteAsync(sqlQuery, new
        {
            entity.Title,
            entity.Symbol,
            entity.Label,
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

        var sqlQuery = $@"DELETE FROM {TableName} WHERE Id = @Id";
        var rowsCount = await db.ExecuteAsync(sqlQuery, new { id });
        return rowsCount > 0;
    }
    #endregion
}
