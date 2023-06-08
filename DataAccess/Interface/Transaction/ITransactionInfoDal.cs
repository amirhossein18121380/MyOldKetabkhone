using Common.Enum;
using DataModel.Models.Transaction;
using DataModel.ViewModel.Transaction;

namespace DataAccess.Interface.Transaction;

public interface ITransactionInfoDal
{
    Task<bool> CardToCardRecordExisted(decimal amount, string fromCard, string trackingCode);
    Task<long> ChargeWallet(TransactionInfo transaction, bool newTransaction = false);
    Task<long> DecreaseWallet(long userId, long amount, string? comment = null, long? creatorUserId = null);
    Task<TransactionInfo?> GetById(long id);
    Task<TransactionInfo?> GetByRefCode(string refCode);
    Task<TransactionInfo?> GetByReserveNumber(long reserveNumber);
    Task<TransactionInfo?> GetByToken(string token);
    Task<(List<GetDepositResultViewModel> data, int totalCount)> GetDeposits(TransactionFilterViewModel filterModel);
    string GetInsertQuery();
    string GetUpdateQuery();
    Task<int> GetUserDepositCount(long userId);
    Task<(List<TransactionInfoViewModel>? data, int totalCount)> GetUserTransaction(TransactionFilterViewModel filterModel);
    Task<(List<UserTransactionReportViewModel> data, int totalCount)> GetUserTransactionReport(GetTransactionReportFilterViewModel filterModel, long userId, params short[] operationTypes);
    Task<long> IncreaseWalletAsync(long userId, long amount, PaymentMethodEnum paymentMethod, long creatorUserId, string? comment = null);
    Task<long> Insert(TransactionInfo entity);
    Task<bool> IsGetRegistrationGift(long userId);
    Task<long> ReverseChargeWallet(TransactionInfo transaction);
    Task<long> Update(TransactionInfo entity);
}