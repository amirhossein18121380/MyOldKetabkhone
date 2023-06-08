using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.Transaction;
using DataAccess.Tool;
using DataModel.Models.Transaction;
using DataModel.ViewModel.Transaction;

namespace DataAccess.DAL.Transaction;

public class GatewayDal : IGatewayDal
{
    #region DataMember
    private const string TableName = @"Transaction.Gateway";
    #endregion

    #region Fetch
    public async Task<List<GatewayCurrencyViewModel>> GetList()
    {
        using var db = new DbEntityObject().GetConnectionString();

        return (await db.QueryAsync<GatewayCurrencyViewModel>($@"SELECT 
                                                                              gtw.Id
                                                                             ,gtw.Title
                                                                             ,gtw.PaymentMethod
                                                                             ,gtw.MerchantId
                                                                             ,gtw.TerminalNumber
                                                                             ,gtw.ImageId
                                                                             ,gtw.MinAmount
                                                                             ,gtw.MaxAmount
                                                                             ,gtw.IsActive
	                                                                         ,cu.Title AS SourceCurrencyTitle
																			 ,dcu.Title AS DestinationCurrencyTitle
                                                                             ,cur.RateValue
                                                                         FROM {TableName} AS gtw
                                                                         INNER JOIN Transaction.CurrencyRate AS cur ON gtw.CurrencyRateId = cur.Id
                                                                         INNER JOIN Transaction.Currency AS cu ON cur.SourceCurrencyId = cu.Id
																		 INNER JOIN Transaction.Currency AS dcu ON cur.DestinationCurrencyId = dcu.Id")).ToList();
    }

    public async Task<List<GatewayCurrencyViewModel>> GetActiveList()
    {
        using var db = new DbEntityObject().GetConnectionString();

        return (await db.QueryAsync<GatewayCurrencyViewModel>($@"SELECT 
                                                                              gtw.Id
                                                                             ,gtw.Title
                                                                             ,gtw.PaymentMethod
                                                                             ,gtw.MerchantId
                                                                             ,gtw.TerminalNumber
                                                                             ,gtw.ImageId
                                                                             ,gtw.MinAmount
                                                                             ,gtw.MaxAmount
                                                                             ,gtw.IsActive
	                                                                         ,cu.Title AS SourceCurrencyTitle
																			 ,dcu.Title AS DestinationCurrencyTitle
                                                                             ,cur.RateValue
                                                                         FROM {TableName} AS gtw
                                                                         INNER JOIN Transaction.CurrencyRate AS cur ON gtw.CurrencyRateId = cur.Id
                                                                         INNER JOIN Transaction.Currency AS cu ON cur.SourceCurrencyId = cu.Id
																		 INNER JOIN Transaction.Currency AS dcu ON cur.DestinationCurrencyId = dcu.Id
                                                                         WHERE gtw.IsActive = true")).ToList();
    }

    public async Task<Gateway?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<Gateway>($@"SELECT * FROM {TableName} WHERE Id = @id",
            new { id });

        return result.SingleOrDefault();
    }

    public async Task<GatewayCurrencyViewModel?> GetViewModelById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<GatewayCurrencyViewModel>($@"SELECT 
                                                                              gtw.Id
                                                                             ,gtw.Title
                                                                             ,gtw.PaymentMethod
                                                                             ,gtw.MerchantId
                                                                             ,gtw.TerminalNumber
                                                                             ,gtw.ImageId
                                                                             ,gtw.MinAmount
                                                                             ,gtw.MaxAmount
                                                                             ,gtw.IsActive
	                                                                         ,cu.Title AS SourceCurrencyTitle
																			 ,dcu.Title AS DestinationCurrencyTitle
                                                                             ,cur.RateValue
                                                                         FROM {TableName} AS gtw
                                                                         INNER JOIN Transaction.CurrencyRate AS cur ON gtw.CurrencyRateId = cur.Id
                                                                         INNER JOIN Transaction.Currency AS cu ON cur.SourceCurrencyId = cu.Id
																		 INNER JOIN Transaction.Currency AS dcu ON cur.DestinationCurrencyId = dcu.Id
                                                                         WHERE gtw.Id = @id",
            new { id });

        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(Gateway entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@Title", entity.Title);
        prams.Add("@PaymentMethod", entity.PaymentMethod);
        prams.Add("@MerchantId", entity.MerchantId);
        prams.Add("@TerminalNumber", entity.TerminalNumber);
        prams.Add("@ImageId", entity.ImageId);
        prams.Add("@ImageName", entity.ImageName);
        prams.Add("@MinAmount", entity.MinAmount);
        prams.Add("@MaxAmount", entity.MaxAmount);
        prams.Add("@CurrencyRateId", entity.CurrencyRateId);
        prams.Add("@IsActive", entity.IsActive);
        prams.Add("@Comment", entity.Comment);
        prams.Add("@CreateOn", entity.CreateOn);

        var entityId = (await db.QueryAsync<long>(
            $@"INSERT INTO {TableName} 
                               (
                                       Title
                                      ,PaymentMethod
                                      ,MerchantId
                                      ,TerminalNumber
                                      ,ImageId
                                      ,ImageName
                                      ,MinAmount
                                      ,MaxAmount
                                      ,CurrencyRateId
                                      ,IsActive
                                      ,Comment
                                      ,CreateOn
                               )
                               VALUES
                               (
                                       @Title
                                      ,@PaymentMethod
                                      ,@MerchantId
                                      ,@TerminalNumber
                                      ,@ImageId
                                      ,@ImageName
                                      ,@MinAmount
                                      ,@MaxAmount
                                      ,@CurrencyRateId
                                      ,@IsActive
                                      ,@Comment
                                      ,@CreateOn
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT);", prams)).SingleOrDefault();

        return entityId;
    }
    #endregion

    #region Update
    public async Task<int> Update(Gateway entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        Title = @Title
                                       ,PaymentMethod = @PaymentMethod
                                       ,MerchantId = @MerchantId
                                       ,TerminalNumber = @TerminalNumber
                                       ,ImageId = @ImageId
                                       ,ImageName = @ImageName
                                       ,MinAmount = @MinAmount
                                       ,MaxAmount = @MaxAmount
                                       ,CurrencyRateId = @CurrencyRateId
                                       ,IsActive = @IsActive
                                       ,Comment = @Comment
                                       ,CreateOn = @CreateOn
                                   WHERE Id = @Id";

        var rowsAffected = await db.ExecuteAsync(sqlQuery, new
        {
            entity.Title,
            entity.PaymentMethod,
            entity.MerchantId,
            entity.TerminalNumber,
            entity.ImageId,
            entity.ImageName,
            entity.MinAmount,
            entity.MaxAmount,
            entity.CurrencyRateId,
            entity.IsActive,
            entity.Comment,
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
