using Common.Enum;
using Common.Helper;
using Dapper;
using DataAccess.Interface.Transaction;
using DataAccess.Tool;
using DataModel.Models.Transaction;

namespace DataAccess.DAL.Transaction;

public class DepositRequestDal : IDepositRequestDal
{
    #region DataMember
    private const string TableName = @"Transaction.DepositRequest";
    #endregion

    #region Fetch
    public async Task<List<DepositRequest>> GetList()
    {
        using var db = new DbEntityObject().GetConnectionString();

        return (await db.QueryAsync<DepositRequest>($@"Select * From {TableName}")).ToList();
    }
    #endregion

    #region InsertRequest
    public async Task<long> InsertRequest(DepositRequest entity)
    {
        using var connection = new DbEntityObject().GetConnectionString();
        using var tran = connection.BeginTransaction();

        try
        {

            #region Get User Main Wallet
            var walletDal = new WalletDal();
            var walletModel = await walletDal.GetUserMainWallet(entity.UserId);
            var walletId = walletModel?.Id;
            if (walletModel == null)
            {
                var wallet = new Wallet
                {
                    CreateOn = DateTime.Now,
                    EntityId = entity.UserId,
                    EntityType = (short)WalletEntityTypeEnum.User,
                    LastBalance = 0,
                    WalletType = (short)WalletTypeEnum.MainWallet
                };

                var rowAffect = await walletDal.Insert(wallet);

                if (rowAffect > 0)
                    walletId = wallet.Id;

                if (!walletId.HasValue)
                {
                    return 0;
                }
            }
            #endregion

            #region Insert TransactionInfo
            var transactionInsertSql = new TransactionInfoDal().GetInsertQuery();
            var reserveNumber = TransactionHelper.GenerateReserveNumber();

            var transactionId = (await connection.QueryAsync<long>(transactionInsertSql,
                new TransactionInfo
                {
                    Amount = entity.Amount,
                    CreateOn = DateTime.Now,
                    OperationType = (short)TransactionOperationTypeEnum.IncreaseWallet,
                    PaymentMethod = (short)PaymentMethodEnum.ManualCardToCard,
                    StatusType = (short)TransactionStatusType.Reserve,
                    UserId = entity.UserId,
                    WalletId = walletModel.Id,
                    TrCode = TransactionHelper.GenerateTrCode(),
                    ReserveNumber = reserveNumber,
                    VerifyDate = entity.VerifyDate,
                }, tran)).SingleOrDefault();

            if (transactionId <= 0)
            {
                tran.Rollback();
                return 0;
            }

            #endregion

            #region InsertRequest
            var depositInsert = GetInsertQuery();
            var depositId = (await connection.QueryAsync<long>(depositInsert,
               new DepositRequest
               {
                   UserId = entity.UserId,
                   Amount = entity.Amount,
                   DepositType = entity.DepositType,
                   TraceNumber = entity.TraceNumber,
                   AccountIdentity = entity.AccountIdentity,
                   StatusType = (short)TransactionStatusType.Reserve,
                   VerifyDate = entity.VerifyDate,
                   CreateOn = DateTime.Now
               }, tran)).SingleOrDefault();

            if (depositId <= 0)
            {
                tran.Rollback();
                return 0;
            }
            #endregion

            tran.Commit();

            return depositId;
        }
        catch (Exception exp)
        {
            tran.Rollback();
            LogHelper.ErrorLog("DepositRequestDal|InsertRequest", exp);
            return 0;
        }
    }

    public string GetInsertQuery()
    {
        return $@"INSERT INTO {TableName} 
                               (
                                   UserId
                                  ,Amount
                                  ,DepositType
                                  ,TraceNumber
                                  ,AccountIdentity
                                  ,VerifyDate
                                  ,CreateOn
                               ) 
                               VALUES
                               (
                                   @UserId                                  
                                  ,@Amount
                                  ,@DepositType
                                  ,@TraceNumber
                                  ,@AccountIdentity
                                  ,@VerifyDate                                 
                                  ,@CreateOn

                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT);";
    }
    #endregion
}
