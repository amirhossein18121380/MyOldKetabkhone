using Common.Helper;
using DataAccess.Interface.Security;
using DataModel.Models.Security;
using DataModel.ViewModel.Common;
using DataModel.ViewModel.Security;
using Microsoft.AspNetCore.Mvc;
using MyApi.Controllers.Common;

namespace MyApi.Controllers.Security;

[Route("api/[controller]")]
[ApiController]
public class MessageController : BaseController
{
    #region constructor
    private readonly IMessagesDal _messages;

    public MessageController(IMessagesDal messages)
    {
        _messages = messages;
    }
    #endregion

    #region PostMessage
    [HttpPost]
    [Route("PostMessage")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult<bool>> PostMessage(PostMessageViewModel data)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (data.UserId <= 0 || string.IsNullOrEmpty(data.Subject?.Trim()) || string.IsNullOrEmpty(data.MessageContent?.Trim()))
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Save Message

            var messageModel = new Messages
            {
                UserId = data.UserId,
                Subject = data.Subject?.Trim(),
                MessageContent = data.MessageContent?.Trim(),
                CreatorId = UserId,
                IsDeleted = false,
                IsRead = false,
                CreateOn = DateTime.Now
            };

            var messageId = await _messages.Insert(messageModel);

            if (messageId <= 0)
                return false;
            #endregion

            #region Send Notification From SignalR
            var unreadMessageCount = await _messages.GetUserUnreadMessageCount(data.UserId);
            var notification = new NotificationViewModel
            {
                Id = messageId,
                Subject = data.Subject?.Trim(),
                UnreadCount = unreadMessageCount
            };

            //await _gameHub.Clients.Group(GroupHelper.GetUserGroupName(data.UserId)).Notification(notification);

            return true;
            #endregion
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("MessageController|PostMessage",ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region GetMessage
    //Service For Site User
    [HttpPost]
    [Route("GetNotification")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult<PagedResponse<List<Messages>>>> GetNotification(PaginationData data)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }
            #endregion

            #region Get Message

            var messages = await _messages.GetListByUserId(UserId, data.PageSize, data.PageNumber);

            var result = new PagedResponse<List<Messages>>(messages.data, messages.totalCount);
            return result;
            #endregion
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("MessageController|GetNotification",ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }

    //Service For Panel User
    [HttpPost]
    [Route("GetMessage")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult<PagedResponse<List<Messages>>>> GetMessage(long userId, PaginationData data)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (userId <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Get Message
            var messages = await _messages.GetListByUserId(userId, data.PageSize, data.PageNumber);

            var result = new PagedResponse<List<Messages>>(messages.data, messages.totalCount);
            return result;
            #endregion
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("MessageController|GetMessage", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region ReadNotification
    [HttpPost]
    [Route("ReadNotification")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult<int>> ReadNotification(long notificationId)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (notificationId <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Get Message
            await _messages.UpdateMessageIsRead(notificationId);

            var count = await _messages.GetUserUnreadMessageCount(UserId);
            return count;
            #endregion
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("MessageController|ReadNotification", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region EditMessage
    [HttpPost]
    [Route("EditMessage")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult<Messages>> EditMessage(Messages data)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (data.Id <= 0 || data.UserId <= 0 || string.IsNullOrEmpty(data.Subject?.Trim()) || string.IsNullOrEmpty(data.MessageContent?.Trim()))
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Update Message
            var messageModel = await _messages.GetById(data.Id);

            if (messageModel == null)
            {
                return HttpHelper.NotFoundContent("Not Found Content Message");
            }

            messageModel.Subject = data.Subject.Trim();
            messageModel.MessageContent = data.MessageContent.Trim();
            messageModel.IsRead = false;
            messageModel.ReadDate = null;

            var messageId = await _messages.Update(messageModel);

            if (messageId <= 0)
                return HttpHelper.FailedContent("Error In Save Record In Db");
            #endregion

            #region Send Notification From SignalR
            var unreadMessageCount = await _messages.GetUserUnreadMessageCount(data.UserId);
            var notification = new NotificationViewModel
            {
                Id = messageModel.Id,
                Subject = data.Subject.Trim(),
                UnreadCount = unreadMessageCount
            };

            //await _gameHub.Clients.Group(GroupHelper.GetUserGroupName(data.UserId)).Notification(notification);

            return messageModel;
            #endregion
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("MessageController|EditMessage", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region DeleteMessage
    [HttpPost]
    [Route("DeleteMessage")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult<bool>> DeleteMessage(long messageId)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (messageId <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            return await _messages.UpdateIsDeleted(messageId);
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("MessageController|DeleteMessage", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion
}
