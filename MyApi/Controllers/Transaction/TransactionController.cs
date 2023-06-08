using Common.Enum;
using Common.Extension;
using Common.Helper;
using DataAccess.Interface.Security;
using DataAccess.Interface.Transaction;
using DataModel.Models.Transaction;
using DataModel.ViewModel.Common;
using DataModel.ViewModel.Transaction;
using Microsoft.AspNetCore.Mvc;
using MyApi.Controllers.Common;

namespace MyApi.Controllers.Transaction;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : BaseController
{
    #region Constructor
    private readonly ITransactionInfoDal _transactionInfo;
    private readonly IWalletDal _wallet;
    private readonly IDepositRequestDal _depositRequest;
    private readonly IGatewayDal _gateway;
    //private readonly IInviteHistory _inviteHistory;
    //private readonly IMessages _messages;
    private readonly IUser _user;

    public TransactionController(ITransactionInfoDal transactionInfo, IWalletDal wallet, 
        IDepositRequestDal depositRequest, IGatewayDal gateway, IUser user)
    {
        _transactionInfo = transactionInfo;
        _wallet = wallet;
        _depositRequest = depositRequest;
        _gateway = gateway;
        _user = user;
        //_inviteHistory = inviteHistory;
        //_messages = messages;
    }
    #endregion

    #region GetDeposits
    [HttpPost]
    [Route("GetDeposits")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult<PagedResponse<List<GetDepositResultViewModel>>>> GetDeposits(TransactionFilterViewModel data)
    {
        try
        {
            var transaction = await _transactionInfo.GetDeposits(data);

            var result = new PagedResponse<List<GetDepositResultViewModel>>(transaction.data, transaction.totalCount);
            return result;
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("TransactionController|GetDeposits", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region GetDepositRequests
    [HttpGet]
    [Route("GetDepositRequests")]
    public async Task<ActionResult<List<DepositRequest>>> GetDepositRequests()
    {
        try
        {
            var depositRequests = await _depositRequest.GetList();

            return depositRequests.Select(c => new DepositRequest
            {
                Id = c.Id,
                UserId = c.UserId,
                Amount = c.Amount,
                DepositType = c.DepositType,
                TraceNumber = c.TraceNumber,
                AccountIdentity = c.AccountIdentity,
                VerifyDate = c.VerifyDate,
            }).ToList();
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("TransactionController|GetDepositRequests", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region DepositRequest
    [HttpPost]
    [Route("DepositRequest")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> DepositRequest(DepositRequest data)
    {

        if (data.UserId <= 0 || data.Amount <= 0 || data.TraceNumber <= 0)
        {
            return HttpHelper.InvalidContent();
        }

        var reqId = await _depositRequest.InsertRequest(data);

        if (reqId <= 0)
        {
            LogHelper.ErrorLog("TransactionController|DepositRequest||Error in submitting deposit request");
            return HttpHelper.InvalidContent();
        }

        return Ok(reqId);
    }
    #endregion

    #region FastDeposit
    [HttpPost]
    [Route("FastDeposit")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> FastDeposit(long userId, long amount)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (userId <= 0 || amount <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            var lastBalance = await _transactionInfo.IncreaseWalletAsync(userId, amount, PaymentMethodEnum.FastDeposit, UserId, "Fast Deposit");

            if (lastBalance <= 0)
            {
                LogHelper.ErrorLog("TransactionController|GetPaymentUrl||Error in wallet fast charge");
                return HttpHelper.InvalidContent();
            }

            #region Invitation Gift

            //if (await CheckUserCompleteValidation(UserId))
            //{
            //    var inviteHistory = await _inviteHistory.GetHistoryRecord(userId);

            //    if (inviteHistory != null)
            //    {
            //        var promotionSettings = await _availableData.GetListByDomain("PromotionSetting");

            //        if (promotionSettings.Any())
            //        {
            //            var totalInvitationGiftCount = promotionSettings
            //                .FirstOrDefault(c => c.DomainKey == "TotalInvitationGiftCount")?.DomainValue;

            //            var invitationGift = promotionSettings.FirstOrDefault(c => c.DomainKey == "InvitationGift");

            //            long.TryParse(invitationGift?.DomainValue, out var invitationGiftAmount);
            //            int.TryParse(totalInvitationGiftCount, out var totalSettingCount);

            //            var totalGiftCount = await _inviteHistory.GetGiftCount(inviteHistory.ParentUserId);

            //            if (invitationGiftAmount > 0 && totalGiftCount <= totalSettingCount)
            //            {
            //                var depCount = await _transactionInfo.GetUserDepositCount(userId);

            //                if (depCount == 1) //if first charge
            //                {
            //                    lastBalance = await _transactionInfo.IncreaseWalletAsync(inviteHistory.ParentUserId,
            //                        invitationGiftAmount,
            //                        PaymentMethodEnum.InvitationGift, userId, "Gift for invitation");

            //                    inviteHistory.IsGetGift = true;
            //                    inviteHistory.GiftAmount = invitationGiftAmount;
            //                    inviteHistory.GiftDate = DateTime.Now;

            //                    await _inviteHistory.Update(inviteHistory);

            //                    var messageModel = new Messages
            //                    {
            //                        UserId = inviteHistory.ParentUserId,
            //                        CreateOn = DateTime.Now,
            //                        CreatorId = 1,
            //                        IsDeleted = false,
            //                        IsRead = false,
            //                        Subject = invitationGift?.Title,
            //                        MessageContent = invitationGift?.Comment
            //                    };

            //                    var messageId = await _messages.Insert(messageModel);

            //                    #region Send Notification From SignalR

            //                    var unreadMessageCount = await _messages.GetUserUnreadMessageCount(messageModel.UserId);
            //                    var notification = new NotificationViewModel
            //                    {
            //                        Id = messageId,
            //                        Subject = messageModel.Subject,
            //                        UnreadCount = unreadMessageCount
            //                    };

            //                    await _gameHub.Clients.Group(GroupHelper.GetUserGroupName(messageModel.UserId))
            //                        .Notification(notification);

            //                    await _gameHub.Clients.Group(GroupHelper.GetUserGroupName(messageModel.UserId))
            //                        .UpdateBalance(lastBalance);

            //                    #endregion
            //                }
            //            }
            //        }
            //    }
            //}
            #endregion

            return Ok(lastBalance);
        }
        catch (Exception ex)
        {
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region FastDebit
    [HttpPost]
    [Route("FastDebit")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> FastDebit(long userId, long amount, string comment)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (userId <= 0 || amount <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            comment = !string.IsNullOrEmpty(comment.Trim())
                ? comment.Trim()
                : "Fast Debit From Panel";
            var lastBalance = await _transactionInfo.DecreaseWallet(userId, amount, comment, UserId);


            return Ok(lastBalance);
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("TransactionController|FastDebit",ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region Transaction Report
    [HttpPost]
    [Route("GetTransactionReport")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult<PagedResponse<List<GetTransactionReportViewModel>>>> GetTransactionReport(GetTransactionReportFilterViewModel data)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }
            #endregion

            #region Get Transaction  
            var transactions = await _transactionInfo.GetUserTransactionReport(data, UserId,
                (short)TransactionOperationTypeEnum.IncreaseWallet,
                (short)TransactionOperationTypeEnum.Withdrawal);

            var operationTypes = EnumExtension.EnumToList<TransactionOperationTypeEnum>();
            var transactionStatusTypes = EnumExtension.EnumToList<TransactionStatusType>();

            var retModel = transactions.data.Select(c => new GetTransactionReportViewModel
            {
                Id = c.Id,
                Amount = c.Amount,
                TrDate = c.CreateOn,
                TrCode = c.TrCode,
                StatusType = c.StatusType switch
                {
                    2 => 1, //success
                    1 => 2, //pending
                    _ => 0 //fail
                },
                OperationType = c.OperationType,
                Comment = c.Comment,
                OperationTitle = operationTypes.FirstOrDefault(op => op.IntValueMember == c.OperationType)?.DisplayMember,
                StatusTitle = c.OperationType == 4
                    ? GetWithdrawalStatusTitle(c.WithdrawalStatus)
                    : transactionStatusTypes.FirstOrDefault(st => st.IntValueMember == c.StatusType)?.DisplayMember,
                DocType = (short)(c.OperationType == (short)TransactionOperationTypeEnum.IncreaseWallet ? 1 : 0)
            }).ToList();

            var result = new PagedResponse<List<GetTransactionReportViewModel>>(retModel, transactions.totalCount);
            return result;
            #endregion
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog(ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }

    private string GetWithdrawalStatusTitle(int? statusType)
    {
        if (!statusType.HasValue)
        {
            return "Incomplete";
        }

        switch (statusType.Value)
        {
            case 0:
                return "Submit Request";
            case 1:
                return "Pending";
            case 2:
            case 5:
                return "Successful";
            case 3:
                return "Failed";
            case 4:
                return "Cancel";
        }

        return "Incomplete";
    }

    [HttpPost]
    [Route("GetUserTransactions")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult<PagedResponse<List<TransactionInfoViewModel>>>> GetUserTransactions(TransactionFilterViewModel data)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (data.UserId <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Get Transaction  
            var transactions = await _transactionInfo.GetUserTransaction(data);

            if (transactions.data == null)
                return HttpHelper.InvalidContent();

            var result = new PagedResponse<List<TransactionInfoViewModel>>(transactions.data, transactions.totalCount);
            return result;
            #endregion
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("TransactionController|GetUserTransactions", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region Gateway
    [HttpGet]
    [Route("GetActiveGateway")]
    public async Task<ActionResult<List<GatewayViewModel>>> GetActiveGateway()
    {
        try
        {
            var gateways = await _gateway.GetActiveList();

            return gateways.Select(c => new GatewayViewModel
            {
                Id = c.Id,
                Title = c.Title,
                ImageId = c.ImageId,
                MinAmount = c.MinAmount,
                MaxAmount = c.MaxAmount,
                CurrencyTitle = c.SourceCurrencyTitle
            }).ToList();
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("TransactionController|GetActiveGateway", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion
}