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

public class WithdrawalDal : IWithdrawalDal
{
    #region DataMember
    private const string TableName = @"Transaction.Withdrawal";
    #endregion

    #region Fetch
    public async Task<(List<GetWithdrawalListViewModel> data, int totalCount)> GetList(WithdrawalGetListFilterViewModel filterModel)
    {
        using var db = new DbEntityObject().GetConnectionString();

        #region Set Where Param
        var prams = new DynamicParameters();

        var whereQuery = string.Empty;

        if (filterModel.UserId.HasValue)
        {
            whereQuery += @"AND withd.UserId = @UserId ";
            prams.Add("UserId", filterModel.UserId.Value);
        }

        if (filterModel.UserName != null && !string.IsNullOrEmpty(filterModel.UserName.Trim()))
        {
            whereQuery += @"AND LOWER(usr.UserName) LIKE @UserName ";
            prams.Add("UserName", $"%{filterModel.UserName.Trim().ToLower()}%");
        }

        if (filterModel.StatusType.HasValue)
        {
            whereQuery += @"AND withd.StatusType = @StatusType ";
            prams.Add("StatusType", filterModel.StatusType.Value);
        }

        if (filterModel.Amount.HasValue)
        {
            whereQuery += @"AND withd.Amount = @Amount ";
            prams.Add("Amount", filterModel.Amount.Value);
        }

        if (filterModel.AccountType.HasValue)
        {
            whereQuery += @"AND withd.AccountType = @AccountType ";
            prams.Add("AccountType", filterModel.AccountType.Value);
        }

        if (filterModel.AccountValue != null && !string.IsNullOrEmpty(filterModel.AccountValue.Trim()))
        {
            whereQuery += @"AND LOWER(withd.AccountValue) LIKE @AccountValue ";
            prams.Add("AccountValue", $"%{filterModel.AccountValue.Trim().ToLower()}%");
        }

        if (filterModel.Date.HasValue)
        {
            whereQuery += @"AND DATE_TRUNC('day', CreateOn) = @CreateOn ";
            prams.Add("CreateOn", filterModel.Date.Value);
        }


        whereQuery = whereQuery.StartsWith("AND") ? $"WHERE{whereQuery.Substring(3, whereQuery.Length - 3)}" : whereQuery;
        #endregion

        #region Sql Query
        var skip = 0;
        if (filterModel.PageNumber > 0)
        {
            skip = filterModel.PageNumber * filterModel.PageSize;
        }

        prams.Add("Skip", skip);

        var sqlQuery = $@"SELECT 
                                         withd.Id
                                        ,withd.TransactionId
                                        ,withd.UserId
                                        ,withd.StatusType
                                        ,withd.Amount
                                        ,withd.AccountType
                                        ,withd.AccountValue
                                        ,withd.BankId
                                        ,withd.FullName
                                        ,withd.ModifierId
                                        ,withd.ModifyDateTime
                                        ,withd.Comment
                                        ,withd.CreateOn
                                  	    ,usr.UserName
                                  	    ,modiUsr.UserName AS ModifierUserName
                                  FROM {TableName} AS withd
                                  INNER JOIN Security.User AS usr ON withd.UserId = usr.Id
                                  LEFT JOIN Security.User AS modiUsr ON withd.ModifierId = modiUsr.Id
                                  {whereQuery}
                                  ORDER BY withd.Id DESC OFFSET @Skip ROWS FETCH NEXT {filterModel.PageSize} ROWS ONLY;

                                  Select COUNT(1) 
                                  FROM {TableName} AS withd
                                  INNER JOIN Security.User AS usr ON withd.UserId = usr.Id
                                  LEFT JOIN Security.User AS modiUsr ON withd.ModifierId = modiUsr.Id
                                  {whereQuery}";


        #endregion

        #region Get Data From Db
        using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);

        var data = (await multiData.ReadAsync<GetWithdrawalListViewModel>()).ToList();
        var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

        return (data, totalCount);
        #endregion
    }

    public async Task<Withdrawal?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<Withdrawal>($@"Select * From {TableName} WHERE Id = @id",
            new { id });

        return result.SingleOrDefault();
    }

    public async Task<int> GetUnseenWithdrawalCount()
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<int>($@"SELECT COUNT(1) FROM {TableName} WHERE StatusType = 0");

        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(Withdrawal entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@TransactionId", entity.TransactionId);
        prams.Add("@UserId", entity.UserId);
        prams.Add("@StatusType", entity.StatusType);
        prams.Add("@Amount", entity.Amount);
        prams.Add("@AccountType", entity.AccountType);
        prams.Add("@AccountValue", entity.AccountValue);
        prams.Add("@BankId", entity.BankId);
        prams.Add("@FullName", entity.FullName);
        prams.Add("@ModifierId", entity.ModifierId);
        prams.Add("@ModifyDateTime", entity.ModifyDateTime);
        prams.Add("@ExchangeAmount", entity.ExchangeAmount);
        prams.Add("@ReferenceNumber", entity.ReferenceNumber);
        prams.Add("@VoucherNumber", entity.VoucherNumber);
        prams.Add("@VoucherCode", entity.VoucherCode);
        prams.Add("@Comment", entity.Comment);
        prams.Add("@CreateOn", entity.CreateOn);

        var entityId = (await db.QueryAsync<long>(GetInsertQuery(), prams)).SingleOrDefault();

        return entityId;
    }

    public string GetInsertQuery()
    {
        return $@"INSERT INTO {TableName} 
                               (
                                   TransactionId
                                  ,UserId
                                  ,StatusType
                                  ,Amount
                                  ,AccountType
                                  ,AccountValue
                                  ,BankId
                                  ,FullName
                                  ,ModifierId
                                  ,ModifyDateTime
                                  ,ExchangeAmount
                                  ,ReferenceNumber
                                  ,VoucherNumber
                                  ,VoucherCode
                                  ,Comment
                                  ,CreateOn
                               ) 
                               VALUES
                               (
                                   @TransactionId                                  
                                  ,@UserId                                  
                                  ,@StatusType
                                  ,@Amount
                                  ,@AccountType
                                  ,@AccountValue
                                  ,@BankId
                                  ,@FullName
                                  ,@ModifierId
                                  ,@ModifyDateTime
                                  ,@ExchangeAmount
                                  ,@ReferenceNumber
                                  ,@VoucherNumber
                                  ,@VoucherCode
                                  ,@Comment
                                  ,@CreateOn

                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT);";
    }
    #endregion

    #region Update
    public async Task<int> Update(Withdrawal entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var rowsAffected = await db.ExecuteAsync(GetUpdateQuery(), new
        {
            entity.TransactionId,
            entity.UserId,
            entity.StatusType,
            entity.Amount,
            entity.AccountType,
            entity.AccountValue,
            entity.BankId,
            entity.FullName,
            entity.ModifierId,
            entity.ModifyDateTime,
            entity.ExchangeAmount,
            entity.ReferenceNumber,
            entity.VoucherNumber,
            entity.VoucherCode,
            entity.Comment,
            entity.CreateOn,
            entity.Id
        });

        return rowsAffected;
    }

    public string GetUpdateQuery()
    {
        return $@"UPDATE {TableName} 
                                   SET 
                                        TransactionId = @TransactionId
                                       ,UserId = @UserId
                                       ,StatusType = @StatusType
                                       ,Amount = @Amount
                                       ,AccountType = @AccountType
                                       ,AccountValue = @AccountValue
                                       ,BankId = @BankId
                                       ,FullName = @FullName
                                       ,ModifierId = @ModifierId
                                       ,ModifyDateTime = @ModifyDateTime
                                       ,ExchangeAmount = @ExchangeAmount
                                       ,ReferenceNumber = @ReferenceNumber
                                       ,VoucherNumber = @VoucherNumber
                                       ,VoucherCode = @VoucherCode
                                       ,Comment = @Comment
                                       ,CreateOn = @CreateOn
                                   WHERE Id = @Id";
    }
    #endregion

    #region Delete
    public async Task<bool> Delete(long id)
    {
        using var db = new DbEntityObject().GetConnectionString(); ;

        var sqlQuery = $@"DELETE FROM {TableName} WHERE Id = @Id";
        var rowsCount = await db.ExecuteAsync(sqlQuery, new { id });
        return rowsCount > 0;
    }
    #endregion

    #region AddWithdrawal
    public async Task<(long id, long lastBalance)> AddWithdrawal(Withdrawal entity, long walletId)
    {
        using var connection = new DbEntityObject().GetConnectionString();
        using var tran = connection.BeginTransaction();

        try
        {
            #region Insert TransactionInfo
            var transactionInsertSql = new TransactionInfoDal().GetInsertQuery();
            var reserveNumber = TransactionHelper.GenerateReserveNumber();

            var transactionId = (await connection.QueryAsync<long>(transactionInsertSql,
                new TransactionInfo
                {
                    Amount = entity.Amount,
                    CreateOn = DateTime.Now,
                    OperationType = (short)TransactionOperationTypeEnum.Withdrawal,
                    StatusType = (short)TransactionStatusType.Reserve,
                    UserId = entity.UserId,
                    WalletId = walletId,
                    TrCode = TransactionHelper.GenerateTrCode(),
                    ReserveNumber = reserveNumber,
                    CreatorId = entity.UserId
                }, tran)).SingleOrDefault();

            if (transactionId <= 0)
            {
                tran.Rollback();
                return (0, -1);
            }
            #endregion

            #region Update Wallet Last Balance
            var walletUpdateQuery = @"UPDATE Transaction.Wallet
                                              SET LastBalance = LastBalance - Cast(@amount as money)
                                          WHERE Id = @id";

            var rowAff = await connection.ExecuteAsync(walletUpdateQuery,
                new
                {
                    amount = entity.Amount,
                    id = walletId
                }, tran);

            if (rowAff <= 0)
            {
                tran.Rollback();
                return (0, -1);
            }
            #endregion

            #region Insert Withdrawal
            var withdrawalInsert = GetInsertQuery();
            var withdrawalId = (await connection.QueryAsync<long>(withdrawalInsert,
                new Withdrawal
                {
                    TransactionId = transactionId,
                    UserId = entity.UserId,
                    StatusType = (short)WithdrawalStatusTypeEnum.UserRequested,
                    Amount = entity.Amount,
                    AccountType = entity.AccountType,
                    AccountValue = entity.AccountValue,
                    BankId = entity.BankId,
                    FullName = entity.FullName,
                    CreateOn = DateTime.Now
                }, tran)).SingleOrDefault();

            if (withdrawalId <= 0)
            {
                tran.Rollback();
                return (0, -1);
            }
            #endregion

            tran.Commit();

            var lastBalance = await new WalletDal().GetUserLastBalance(entity.UserId);
            return (withdrawalId, lastBalance);
        }
        catch (Exception exp)
        {
            tran.Rollback();
            LogHelper.ErrorLog("WithdrawalDal|AddWithdrawal", exp);
            return (0, -1);
        }
    }
    #endregion

    #region VerifyWithdrawal
    public async Task<long> VerifyWithdrawal(Withdrawal entity)
    {
        using var connection = new DbEntityObject().GetConnectionString();
        using var tran = connection.BeginTransaction();

        try
        {
            #region Get TransactionInfo
            var transactionInfoDal = new TransactionInfoDal();
            var transaction = await transactionInfoDal.GetById(entity.TransactionId);

            if (transaction == null)
            {
                return -1;
            }
            #endregion

            #region Update Transaction
            transaction.StatusType = (short)TransactionStatusType.IsOk;
            transaction.VerifyDate = DateTime.Now;

            var transactionStatus = await connection.ExecuteAsync(transactionInfoDal.GetUpdateQuery(),
               transaction, tran);

            if (transactionStatus <= 0)
            {
                tran.Rollback();
                return -1;
            }
            #endregion

            #region Update Withdrawal
            var rowAff = await connection.ExecuteAsync(GetUpdateQuery(), entity, tran);

            if (rowAff <= 0)
            {
                tran.Rollback();
                return -1;
            }
            #endregion

            tran.Commit();

            var lastBalance = await new WalletDal().GetUserLastBalance(entity.UserId);
            return lastBalance;
        }
        catch (Exception exp)
        {
            tran.Rollback();
            LogHelper.ErrorLog("WithdrawalDal|VerifyWithdrawal", exp);
            return -1;
        }
    }
    #endregion

    #region ReverseWithdrawal
    public async Task<long> ReverseWithdrawal(Withdrawal entity)
    {
        using var connection = new DbEntityObject().GetConnectionString();
        using var tran = connection.BeginTransaction();

        try
        {
            #region Get TransactionInfo
            var transactionInfoDal = new TransactionInfoDal();
            var transaction = await transactionInfoDal.GetById(entity.TransactionId);

            if (transaction == null)
            {
                return -1;
            }
            #endregion

            #region Update Transaction
            transaction.StatusType = (short)TransactionStatusType.Failed;
            transaction.VerifyDate = DateTime.Now;

            var transactionStatus = await connection.ExecuteAsync(transactionInfoDal.GetUpdateQuery(),
                transaction, tran);

            if (transactionStatus <= 0)
            {
                tran.Rollback();
                return -1;
            }
            #endregion

            #region Update Withdrawal
            var withdrawalStatus = await connection.ExecuteAsync(GetUpdateQuery(), entity, tran);

            if (withdrawalStatus <= 0)
            {
                tran.Rollback();
                return -1;
            }
            #endregion

            #region Update Wallet Last Balance
            var walletUpdateQuery = @"UPDATE Transaction.Wallet
                                              SET LastBalance = LastBalance + @amount
                                          WHERE Id = @id";

            var rowAff = await connection.ExecuteAsync(walletUpdateQuery,
                new
                {
                    amount = transaction.Amount,
                    id = transaction.WalletId
                }, tran);

            if (rowAff <= 0)
            {
                tran.Rollback();
                return -1;
            }
            #endregion

            tran.Commit();

            var lastBalance = await new WalletDal().GetUserLastBalance(entity.UserId);
            return lastBalance;
        }
        catch (Exception exp)
        {
            tran.Rollback();
            LogHelper.ErrorLog("WithdrawalDal|ReverseWithdrawal", exp);
            return -1;
        }
    }
    #endregion
}
