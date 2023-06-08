using Common.Enum;
using Dapper;
using DataAccess.Interface.Transaction;
using DataAccess.Tool;
using DataModel.Models.Transaction;

namespace DataAccess.DAL.Transaction;

public class WalletDal : IWalletDal
{
    #region DataMember
    private const string TableName = "[Transaction].[Wallet]";
    #endregion

    #region Fetch
    public async Task<Wallet?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Wallet>($@"Select * From {TableName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }

    public async Task<long> GetUserLastBalance(long userId)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Wallet>($@"Select * From {TableName} 
                                                                             WHERE EntityType = @entityType AND
                                                                                   WalletType = @walletType AND
                                                                                   EntityId = @userId",
            new
            {
                entityType = (short)WalletEntityTypeEnum.User,
                walletType = (short)WalletTypeEnum.MainWallet,
                userId
            });

        var wallet = result.SingleOrDefault();

        return wallet != null ? Convert.ToInt64(wallet.LastBalance) : 0;
    }

    public async Task<Wallet?> GetUserMainWallet(long userId)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<Wallet>($@"Select * From {TableName} 
                                                                             WHERE EntityType = @entityType AND
                                                                                   WalletType = @walletType AND
                                                                                   EntityId = @userId",
            new
            {
                entityType = (short)WalletEntityTypeEnum.User,
                walletType = (short)WalletTypeEnum.MainWallet,
                userId
            });

        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public string GetInsertQuery()
    {
        return $@"INSERT INTO {TableName} 
                               (
                                       WalletType
                                      ,EntityId
                                      ,LastBalance
                                      ,EntityType
                                      ,CreateOn
                               )
                               VALUES
                               (
                                       @WalletType
                                      ,@EntityId
                                      ,@LastBalance
                                      ,@EntityType
                                      ,@CreateOn
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT);";
    }

    public async Task<long> Insert(Wallet entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@WalletType", entity.WalletType);
        prams.Add("@EntityId", entity.EntityId);
        prams.Add("@LastBalance", entity.LastBalance);
        prams.Add("@EntityType", entity.EntityType);
        prams.Add("@CreateOn", entity.CreateOn);

        var entityId = (await db.QueryAsync<long>(GetInsertQuery(), prams)).SingleOrDefault();

        return entityId;
    }
    #endregion

    #region Update
    public async Task<long> UpdateLastBalance(long id, decimal lastBalance)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var rowsAffected = await db.ExecuteAsync(GetLastBalanceUpdateQuery(), new
        {
            lastBalance,
            id
        });

        return rowsAffected;
    }

    public string GetLastBalanceUpdateQuery()
    {
        return $@"UPDATE {TableName}
                         SET LastBalance = @lastBalance
                      WHERE Id = @id";
    }
    #endregion
}
