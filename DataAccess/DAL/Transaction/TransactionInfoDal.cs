using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enum;
using Common.Helper;
using Dapper;
using DataAccess.Interface.Transaction;
using DataAccess.Tool;
using DataModel.Models.Transaction;
using DataModel.ViewModel.Transaction;

namespace DataAccess.DAL.Transaction;

public class TransactionInfoDal : ITransactionInfoDal
{
    #region DataMember
    private const string TableName = @"Transaction.TransactionInfo";
    #endregion

    #region Fetch
    public async Task<TransactionInfo?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<TransactionInfo>($@"Select * From {TableName} WHERE Id = @id", new { id });

        return result.SingleOrDefault();
    }

    public async Task<TransactionInfo?> GetByToken(string token)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<TransactionInfo>($@"Select * From {TableName} WHERE Token = @token", new { token });

        return result.SingleOrDefault();
    }

    public async Task<int> GetUserDepositCount(long userId)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<int>($@"Select Count(1) From {TableName} 
                                                             WHERE UserId = @userId AND 
                                                                   OperationType = 1 AND 
                                                                   StatusType = 2 AND 
                                                                   PaymentMethod NOT IN (7,8,11)", new { userId });

        return result.FirstOrDefault();
    }

    public async Task<TransactionInfo?> GetByRefCode(string refCode)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<TransactionInfo>($@"Select * From {TableName} WHERE RefCode = @refCode", new { refCode });

        return result.SingleOrDefault();
    }

    public async Task<TransactionInfo?> GetByReserveNumber(long reserveNumber)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<TransactionInfo>($@"Select * From {TableName} WHERE ReserveNumber = @reserveNumber",
            new { reserveNumber });

        return result.FirstOrDefault();
    }

    public async Task<bool> IsGetRegistrationGift(long userId)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<int>($@"Select Count(1) From {TableName} 
                                                                       WHERE UserId = @userId AND 
                                                                             OperationType = 1 AND 
                                                                             StatusType = 2 AND 
                                                                             PaymentMethod = 7", new { userId });

        return result.FirstOrDefault() > 0;
    }

    public async Task<bool> CardToCardRecordExisted(decimal amount, string fromCard, string trackingCode)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<int>($@"Select Count(1) From {TableName} 
                                                                       WHERE OperationType = 1 AND 
                                                                             StatusType = 2 AND 
                                                                             PaymentMethod = 13 AND
                                                                             Amount = @amount AND 
                                                                             Token = @fromCard AND
                                                                             RefCode = @trackingCode", new { amount, fromCard, trackingCode });

        return result.FirstOrDefault() > 0;
    }

    public async Task<(List<UserTransactionReportViewModel> data, int totalCount)> GetUserTransactionReport(
        GetTransactionReportFilterViewModel filterModel, long userId, params short[] operationTypes)
    {
        using var db = new DbEntityObject().GetConnectionString();

        #region Set Where Param

        var prams = new DynamicParameters();

        var whereQuery = @"WHERE tr.UserId = @UserId ";
        prams.Add("UserId", userId);

        if (operationTypes.Any())
        {
            whereQuery += @"AND tr.OperationType = ANY (@OperationTypes) ";
            prams.Add("OperationTypes", operationTypes.ToArray());
        }

        if (filterModel.FromDate.HasValue)
        {
            whereQuery += @"AND DATE_TRUNC('day', CreateOn) >= @FromDate ";
            prams.Add("FromDate", filterModel.FromDate.Value);
        }

        if (filterModel.ToDate.HasValue)
        {
            whereQuery += @"AND DATE_TRUNC('day', CreateOn) <= @ToDate ";
            prams.Add("ToDate", filterModel.ToDate.Value);
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
                                         tr.Id
                                        ,tr.OperationType
                                        ,tr.StatusType
                                        ,tr.Amount
                                        ,tr.WalletId
                                        ,tr.UserId
                                        ,tr.TrCode
                                        ,tr.RefCode
                                        ,tr.ReserveNumber
                                        ,tr.GatewayId
                                        ,tr.PaymentMethod
                                        ,tr.Token
                                        ,tr.VerifyDate
                                        ,tr.Comment
                                        ,tr.CreatorId
                                        ,tr.CreateOn
	                                    ,wid.StatusType AS WithdrawalStatus
                                  FROM {TableName} As tr
                                  LEFT JOIN Transaction.Withdrawal AS wid ON tr.Id = wid.TransactionId
                                  {whereQuery}
                                  ORDER BY tr.Id DESC OFFSET @Skip ROWS FETCH NEXT {filterModel.PageSize} ROWS ONLY;

                                  Select COUNT(1) 
                                  FROM {TableName} As tr
                                  {whereQuery};";

        #endregion

        #region Get Data From Db

        using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);

        var data = (await multiData.ReadAsync<UserTransactionReportViewModel>()).ToList();
        var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

        return (data, totalCount);

        #endregion
    }
    #endregion

    #region Insert
    public async Task<long> Insert(TransactionInfo entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@OperationType", entity.OperationType);
        prams.Add("@StatusType", entity.StatusType);
        prams.Add("@Amount", entity.Amount);
        prams.Add("@CurrentBalance", entity.CurrentBalance);
        prams.Add("@WalletId", entity.WalletId);
        prams.Add("@UserId", entity.UserId);
        prams.Add("@TrCode", entity.TrCode);
        prams.Add("@RefCode", entity.RefCode);
        prams.Add("@ReserveNumber", entity.ReserveNumber);
        prams.Add("@GatewayId", entity.GatewayId);
        prams.Add("@Comment", entity.Comment);
        prams.Add("@CreatorId", entity.CreatorId);
        prams.Add("@CreateOn", entity.CreateOn);
        prams.Add("@PaymentMethod", entity.PaymentMethod);
        prams.Add("@Token", entity.Token);
        prams.Add("@VerifyDate", entity.VerifyDate);

        var entityId = (await db.QueryAsync<long>(GetInsertQuery(), prams))
            .SingleOrDefault();

        return entityId;
    }

    public string GetInsertQuery()
    {
        return $@"INSERT INTO {TableName}
                              (OperationType
                              ,StatusType
                              ,Amount
                              ,CurrentBalance
                              ,WalletId
                              ,UserId
                              ,TrCode
                              ,RefCode
                              ,ReserveNumber
                              ,GatewayId
                              ,Comment
                              ,CreatorId
                              ,CreateOn
                              ,PaymentMethod
                              ,Token
                              ,VerifyDate)
                        VALUES
                              (@OperationType
                              ,@StatusType
                              ,@Amount
                              ,@CurrentBalance
                              ,@WalletId
                              ,@UserId
                              ,@TrCode
                              ,@RefCode
                              ,@ReserveNumber
                              ,@GatewayId
                              ,@Comment
                              ,@CreatorId
                              ,@CreateOn
                              ,@PaymentMethod
                              ,@Token
                              ,@VerifyDate);
                              SELECT CAST(SCOPE_IDENTITY() as BIGINT);";
    }
    #endregion

    #region Update
    public async Task<long> Update(TransactionInfo entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var rowsAffected = await db.ExecuteAsync(GetUpdateQuery(), new
        {
            entity.OperationType,
            entity.StatusType,
            entity.Amount,
            entity.CurrentBalance,
            entity.WalletId,
            entity.UserId,
            entity.TrCode,
            entity.RefCode,
            entity.ReserveNumber,
            entity.GatewayId,
            entity.Comment,
            entity.CreatorId,
            entity.CreateOn,
            entity.PaymentMethod,
            entity.Token,
            entity.VerifyDate,
            entity.Id
        });

        return rowsAffected;
    }

    public string GetUpdateQuery()
    {
        return $@"UPDATE {TableName} 
                                   SET 
                                        OperationType = @OperationType
                                       ,StatusType = @StatusType
                                       ,Amount = @Amount
                                       ,CurrentBalance = @CurrentBalance
                                       ,WalletId = @WalletId
                                       ,UserId = @UserId
                                       ,TrCode = @TrCode
                                       ,RefCode = @RefCode
                                       ,ReserveNumber = @ReserveNumber
                                       ,GatewayId = @GatewayId
                                       ,Comment = @Comment
                                       ,CreatorId = @CreatorId
                                       ,CreateOn = @CreateOn
                                       ,PaymentMethod = @PaymentMethod
                                       ,Token = @Token
                                       ,VerifyDate = @VerifyDate
                                   WHERE Id = @Id";
    }
    #endregion

    #region Charge Wallet
    public async Task<long> ChargeWallet(TransactionInfo transaction, bool newTransaction = false)
    {
        using var connection = new DbEntityObject().GetConnectionString();
        using var tran = connection.BeginTransaction();

        try
        {
            #region Get User Main Wallet
            var walletDal = new WalletDal();
            var walletModel = await walletDal.GetById(transaction.WalletId);
            if (walletModel == null)
            {
                return 0;
            }
            #endregion

            if (newTransaction)
            {
                #region Insert TransactionInfo
                var transactionInsertSql = GetInsertQuery();

                transaction.CurrentBalance = walletModel.LastBalance + transaction.Amount;

                var transactionId = (await connection.QueryAsync<long>(transactionInsertSql,
                    transaction, tran)).SingleOrDefault();

                if (transactionId <= 0)
                {
                    tran.Rollback();
                    return 0;
                }

                #endregion
            }
            else
            {
                #region Update TransactionInfo
                var transactionUpdateSql = GetUpdateQuery();

                transaction.CurrentBalance = walletModel.LastBalance + transaction.Amount;

                var transactionId = await connection.ExecuteAsync(transactionUpdateSql,
                    transaction, tran);

                if (transactionId <= 0)
                {
                    tran.Rollback();
                    return 0;
                }

                #endregion
            }

            #region Update Wallet Last Balance

            var walletUpdateQuery = @"UPDATE Transaction.Wallet
                                              SET LastBalance = LastBalance + Cast(@Amount as money)
                                          WHERE Id = @Id";

            var rowAff = await connection.ExecuteAsync(walletUpdateQuery,
                new
                {
                    transaction.Amount,
                    walletModel.Id
                }, tran);

            if (rowAff <= 0)
            {
                tran.Rollback();
                return 0;
            }

            #endregion

            tran.Commit();

            var lastBalance = await walletDal.GetUserLastBalance(transaction.UserId);
            return lastBalance;
        }
        catch (Exception exp)
        {
            tran.Rollback();
            LogHelper.ErrorLog("TransactionInfoDal|ChargeWallet", exp);
            return 0;
        }
    }

    public async Task<long> ReverseChargeWallet(TransactionInfo transaction)
    {
        using var connection = new DbEntityObject().GetConnectionString();
        using var tran = connection.BeginTransaction();

        try
        {
            #region Get User Main Wallet
            var walletDal = new WalletDal();
            var walletModel = await walletDal.GetById(transaction.WalletId);
            if (walletModel == null)
            {
                return 0;
            }
            #endregion

            #region Insert TransactionInfo
            var transactionId = await connection.ExecuteAsync(GetUpdateQuery(),
                transaction, tran);

            if (transactionId <= 0)
            {
                tran.Rollback();
                return 0;
            }

            #endregion

            #region Update Wallet Last Balance

            var walletUpdateQuery = @"UPDATE Transaction.Wallet
                                              SET LastBalance = LastBalance - Cast(@Amount as money)
                                          WHERE Id = @Id";

            var rowAff = await connection.ExecuteAsync(walletUpdateQuery,
                new
                {
                    transaction.Amount,
                    walletModel.Id
                }, tran);

            if (rowAff <= 0)
            {
                tran.Rollback();
                return 0;
            }

            #endregion

            tran.Commit();

            var lastBalance = await walletDal.GetUserLastBalance(transaction.UserId);
            return lastBalance;
        }
        catch (Exception exp)
        {
            tran.Rollback();
            LogHelper.ErrorLog("TransactionInfoDal|ReverseChargeWallet", exp);
            return 0;
        }
    }
    #endregion

    #region GetDeposits
    public async Task<(List<GetDepositResultViewModel> data, int totalCount)> GetDeposits(TransactionFilterViewModel filterModel)
    {
        using var db = new DbEntityObject().GetConnectionString();

        #region Set Where Param
        var prams = new DynamicParameters();

        var whereQuery = @"WHERE tr.OperationType = 1 AND tr.PaymentMethod != 7 ";

        if (filterModel.UserId > 0)
        {
            whereQuery += @"AND tr.UserId = @UserId ";
            prams.Add("UserId", filterModel.UserId);
        }

        if (filterModel.UserName != null && !string.IsNullOrEmpty(filterModel.UserName.Trim()))
        {
            whereQuery += @"AND LOWER(usr.UserName) LIKE @UserName ";
            prams.Add("UserName", $"%{filterModel.UserName.Trim().ToLower()}%");
        }

        if (filterModel.PaymentMethod.HasValue)
        {
            whereQuery += @"AND tr.PaymentMethod = @PaymentMethod ";
            prams.Add("@PaymentMethod", filterModel.PaymentMethod);
        }

        if (filterModel.Amount > 0)
        {
            whereQuery += @"AND tr.Amount >= @Amount ";
            prams.Add("@Amount", filterModel.Amount);
        }

        if (filterModel.StatusType > 0)
        {
            whereQuery += @"AND tr.StatusType = @StatusType ";
            prams.Add("@StatusType", filterModel.StatusType);
        }

        if (filterModel.StartDate.HasValue)
        {
            whereQuery += @"AND DATE_TRUNC('day', tr.CreateOn) >= @StartDate ";
            prams.Add("StartDate", filterModel.StartDate.Value);
        }

        if (filterModel.EndDate.HasValue)
        {
            whereQuery += @"AND DATE_TRUNC('day', tr.CreateOn) <= @EndDate ";
            prams.Add("EndDate", filterModel.EndDate.Value);
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
                                         tr.Id AS TransactionId 
                                        ,tr.OperationType
                                        ,tr.StatusType
                                        ,tr.Amount
                                        ,tr.UserId
                                        ,tr.TrCode      
                                        ,tr.PaymentMethod
                                        ,tr.VerifyDate
                                        ,tr.Comment
                                        ,tr.CreatorId
                                        ,tr.CreateOn
                                        ,usr.UserName
                                  	    ,panelUsr.UserName AS CreatorUserName
                                  FROM Transaction.TransactionInfo AS tr
                                  INNER JOIN Security.User AS usr ON tr.UserId = usr.Id
                                  INNER JOIN Security.User AS panelUsr ON tr.CreatorId = panelUsr.Id
                                  {whereQuery}
                                  ORDER BY tr.Id DESC OFFSET @Skip ROWS FETCH NEXT {filterModel.PageSize} ROWS ONLY;

                                  Select COUNT(1) 
                                  FROM {TableName} AS tr
                                  INNER JOIN Security.User AS usr ON tr.UserId = usr.Id
                                  INNER JOIN Security.User AS panelUsr ON tr.CreatorId = panelUsr.Id
                                  {whereQuery}";
        #endregion

        #region Get Data From Db
        using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);

        var data = (await multiData.ReadAsync<GetDepositResultViewModel>()).ToList();
        var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

        return (data, totalCount);
        #endregion
    }
    #endregion

    #region FastDeposit
    public async Task<long> IncreaseWalletAsync(long userId, long amount, PaymentMethodEnum paymentMethod, long creatorUserId, string? comment = null)
    {
        using var connection = new DbEntityObject().GetConnectionString();
        using var tran = connection.BeginTransaction();

        try
        {
            #region Get User Main Wallet
            var walletDal = new WalletDal();
            var walletModel = await walletDal.GetUserMainWallet(userId);
            if (walletModel == null)
            {
                return 0;
            }
            #endregion

            #region Insert TransactionInfo
            var transactionInsertSql = GetInsertQuery();

            var transactionId = (await connection.QueryAsync<long>(transactionInsertSql,
                new TransactionInfo
                {
                    UserId = userId,
                    Amount = amount,
                    CreateOn = DateTime.Now,
                    PaymentMethod = (short)paymentMethod,
                    OperationType = (short)TransactionOperationTypeEnum.IncreaseWallet,
                    StatusType = (short)TransactionStatusType.IsOk,
                    WalletId = walletModel.Id,
                    TrCode = TransactionHelper.GenerateTrCode(),
                    VerifyDate = DateTime.Now,
                    Comment = comment,
                    CreatorId = creatorUserId,
                    CurrentBalance = walletModel.LastBalance + amount
                }, tran)).SingleOrDefault();

            if (transactionId <= 0)
            {
                tran.Rollback();
                return 0;
            }

            #endregion

            #region Update Wallet Last Balance

            var walletUpdateQuery = @"UPDATE Transaction.Wallet
                                              SET LastBalance = LastBalance + Cast(@Amount as money)
                                          WHERE Id = @Id";

            var rowAff = await connection.ExecuteAsync(walletUpdateQuery,
                new
                {
                    Amount = amount,
                    walletModel.Id
                }, tran);

            if (rowAff <= 0)
            {
                tran.Rollback();
                return 0;
            }

            #endregion

            tran.Commit();

            var lastBalance = await walletDal.GetUserLastBalance(userId);
            return lastBalance;
        }
        catch (Exception exp)
        {
            tran.Rollback();
            LogHelper.ErrorLog("TransactionInfoDal|IncreaseWalletAsync", exp);
            return 0;
        }
    }
    #endregion

    #region Decrease Wallet
    public async Task<long> DecreaseWallet(long userId, long amount, string? comment = null, long? creatorUserId = null)
    {
        try
        {
            return await DecreaseWalletMethod(userId, amount, TransactionOperationTypeEnum.DecreaseWallet, comment, creatorUserId);
        }
        catch (Exception exp)
        {
            LogHelper.ErrorLog("TransactionInfoDal|DecreaseWallet", exp);
            return -1;
        }
    }

    private async Task<long> DecreaseWalletMethod(long userId, long amount, TransactionOperationTypeEnum operationType,
        string? comment, long? creatorUserId)
    {
        using var connection = new DbEntityObject().GetConnectionString();
        using var tran = connection.BeginTransaction();
        try
        {
            #region Get User Main Wallet

            var walletDal = new WalletDal();
            var walletModel = await walletDal.GetUserMainWallet(userId);
            if (walletModel == null)
            {
                return -1;
            }

            #endregion

            #region Insert TransactionInfo

            var transactionInsertSql = GetInsertQuery();
            var transactionId = (await connection.QueryAsync<long>(transactionInsertSql,
                new TransactionInfo
                {
                    Amount = amount,
                    CreateOn = DateTime.Now,
                    UserId = userId,
                    OperationType = (short)operationType,
                    StatusType = (short)TransactionStatusType.IsOk,
                    WalletId = walletModel.Id,
                    Comment = comment,
                    CurrentBalance = walletModel.LastBalance - amount,
                    CreatorId = creatorUserId ?? userId
                }, tran)).SingleOrDefault();

            if (transactionId <= 0)
            {
                tran.Rollback();
                return -1;
            }

            #endregion

            #region Update Wallet Last Balance

            var lastBalance = Convert.ToInt64(walletModel.LastBalance - amount);
            var walletUpdateLastBalanceQuery = walletDal.GetLastBalanceUpdateQuery();
            var rowAff = await connection.ExecuteAsync(walletUpdateLastBalanceQuery,
                new
                {
                    lastBalance,
                    id = walletModel.Id
                }, tran);

            if (rowAff <= 0)
            {
                tran.Rollback();
                return -1;
            }

            #endregion

            tran.Commit();

            return lastBalance;
        }

        catch (Exception exp)
        {
            tran.Rollback();
            LogHelper.ErrorLog("TransactionInfoDal|DecreaseWalletMethod", exp);
            return -1;
        }
    }


    #endregion

    #region Report
    public async Task<(List<TransactionInfoViewModel>? data, int totalCount)> GetUserTransaction(TransactionFilterViewModel filterModel)
    {
        using var db = new DbEntityObject().GetConnectionString();

        #region Set Where Param

        if (filterModel.UserId <= 0)
        {
            return (null, 0);
        }

        var prams = new DynamicParameters();

        var whereQuery = @"WHERE UserId = @UserId ";
        prams.Add("UserId", filterModel.UserId);

        if (filterModel.OperationType > 0)
        {
            whereQuery += @"AND OperationType = @OperationType ";
            prams.Add("OperationType", filterModel.OperationType);
        }

        if (filterModel.PaymentMethod.HasValue)
        {
            whereQuery += @"AND PaymentMethod = @PaymentMethod ";
            prams.Add("PaymentMethod", filterModel.PaymentMethod);
        }

        if (filterModel.Amount > 0)
        {
            whereQuery += @"AND Amount >= @Amount ";
            prams.Add("Amount", filterModel.Amount);
        }

        if (filterModel.StatusType > 0)
        {
            whereQuery += @"AND StatusType = @StatusType ";
            prams.Add("StatusType", filterModel.StatusType);
        }

        if (filterModel.StartDate.HasValue)
        {
            whereQuery += @"AND DATE_TRUNC('day', CreateOn) >= @StartDate ";
            prams.Add("StartDate", filterModel.StartDate.Value);
        }

        if (filterModel.EndDate.HasValue)
        {
            whereQuery += @"AND DATE_TRUNC('day', CreateOn) <= @EndDate ";
            prams.Add("EndDate", filterModel.EndDate.Value);
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
                                         Id
                                        ,OperationType
                                        ,StatusType
                                        ,Amount
										,CurrentBalance
										,WalletId
                                        ,UserId
                                        ,TrCode
                                        ,PaymentMethod
                                        ,VerifyDate
                                        ,Comment
                                        ,CreateOn
                                  FROM {TableName}
                                  {whereQuery}
                                  ORDER BY Id DESC OFFSET @Skip ROWS FETCH NEXT {filterModel.PageSize} ROWS ONLY;

                                  Select COUNT(1) 
                                  FROM {TableName}
                                  {whereQuery}";
        #endregion

        #region Get Data From Db
        using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);

        var data = (await multiData.ReadAsync<TransactionInfoViewModel>()).ToList();
        var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

        return (data, totalCount);
        #endregion
    }
    #endregion
}
