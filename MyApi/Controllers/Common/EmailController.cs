using Common.Enum;
using Common.Helper;
using DataAccess.Interface;
using DataAccess.Interface.Security;
using DataModel.Common;
using Microsoft.AspNetCore.Mvc;
using MyApi.Helpers;

namespace MyApi.Controllers.Common;

[Route("api/[controller]")]
[ApiController]
public class EmailController : BaseController
{
    #region Constructor
    private readonly IEmailDal _emailTemplate;
    private readonly IUser _user;
    public EmailController(IEmailDal emailTemplate, IUser user)
    {
        _emailTemplate = emailTemplate;
        _user = user;
    }
    #endregion

    #region GetList
    [HttpGet]
    [Route("GetList")]
    [PkPermission(ResourcesEnum.Email)]
    public async Task<ActionResult<List<EmailTemplate>>> GetList()
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }
            #endregion

            #region Get List
            var emails = await _emailTemplate.GetList();

            return emails;
            #endregion
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("EmailController|GetList", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region GetById
    [HttpGet]
    [Route("GetById/{id}")]
    [PkPermission(ResourcesEnum.Email)]
    public async Task<ActionResult<EmailTemplate>> GetById(long id)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (id <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Get List
            var email = await _emailTemplate.GetById(id);

            if (email == null)
            {
                return HttpHelper.NotFoundContent("Not Found Content Message");
            }

            return email;
            #endregion
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("EmailController|GetById", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region Add
    [HttpPost]
    [Route("Add")]
    [IgnoreAntiforgeryToken]
    [PkPermission(ResourcesEnum.Email)]
    public async Task<ActionResult<long>> Add(EmailTemplate data)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (string.IsNullOrEmpty(data.Subject?.Trim()) || string.IsNullOrEmpty(data.EmailContent?.Trim()) || data.SendType < 0)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Insert To DB
            data.CreateOn = DateTime.Now;
            data.CreatorId = UserId;
            data.IsSend = false;

            var emailTemplateId = await _emailTemplate.Insert(data);

            if (emailTemplateId <= 0)
            {
                return HttpHelper.FailedContent("Error In Save Record In Db");
            }
            #endregion

            return emailTemplateId;
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("EmailController|Add", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region Edit
    [HttpPost]
    [Route("Edit")]
    [IgnoreAntiforgeryToken]
    [PkPermission(ResourcesEnum.Email)]
    public async Task<ActionResult<bool>> Edit(EmailTemplate data)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (data.Id <= 0 || string.IsNullOrEmpty(data.Subject?.Trim()) || string.IsNullOrEmpty(data.EmailContent?.Trim()) || data.SendType < 0)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Insert To DB
            var email = await _emailTemplate.GetById(data.Id);

            if (email == null)
            {
                return HttpHelper.NotFoundContent("Not Found Content Message");
            }

            email.Subject = data.Subject;
            email.EmailContent = data.EmailContent;
            email.SendType = data.SendType;
            email.FilterValue = data.FilterValue;

            var emailTemplateId = await _emailTemplate.Update(email);

            if (emailTemplateId <= 0)
            {
                return HttpHelper.FailedContent("Error In Save Record In Db");
            }
            #endregion

            return true;
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("EmailController|Edit", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region Delete
    [HttpGet]
    [Route("Delete/{id}")]
    [PkPermission(ResourcesEnum.Email)]
    public async Task<ActionResult<bool>> Delete(long id)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (id <= 0)
            {
                return HttpHelper.InvalidContent();
            }

            var email = await _emailTemplate.GetById(id);

            if (email == null)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Delete
            if (email.IsSend)
            {
                return HttpHelper.FailedContent("Can Not Send Email Template");
            }

            var delete = await _emailTemplate.Delete(id);
            return delete;
            #endregion
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("EmailController|Delete", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region Send Email
    [HttpGet]
    [Route("Send/{id}")]
    [PkPermission(ResourcesEnum.Email)]
    public async Task<ActionResult<bool>> Send(long id)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (id <= 0)
            {
                return HttpHelper.InvalidContent();
            }

            var email = await _emailTemplate.GetById(id);

            if (email == null || string.IsNullOrEmpty(email.Subject) || string.IsNullOrEmpty(email.EmailContent))
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Get Emails

            List<string> emails;
            //if ((EmailSendTypeEnum)email.SendType == EmailSendTypeEnum.OutSideUser)
            //{
            //    emails = await _outSideUser.GetAllEmails();

            //    if (!emails.Any())
            //    {
            //        return HttpHelper.FailedContent("Email Not Find");
            //    }
            //}

            emails = await _user.GetUserEmails((EmailSendTypeEnum)email.SendType, email.FilterValue);

            if (!emails.Any())
            {
                return HttpHelper.FailedContent("Not Found Content Message");
            }

            #endregion

            #region Send Emails
            //var testEmails = new List<string> { "rezadoodangi@gmail.com", "rezadoodangi@hotmail.com", "eshghi26amirhossein@gmail.com", "salehgholamian@gmail.com", "mostafa.gholamitousi@gmail.com" };

            //Parallel.ForEach(emails, em =>
            //{
            //    _ = MailHelper.SendEmail(email.Subject, email.EmailContent, @"info@pokernet.vip", em);
            //});

            LogHelper.InfoLog($"EmailController|Send Emails => Email Count: {emails.Count}");
            var totalCount = 0;
            var totalSendCount = 0;
            var totalFailCount = 0;
            var startDate = DateTime.Now;
            //emails.Add("rezadoodangi@gmail.com");
            //emails.Add("eshghi26amirhossein@gmail.com");
            //emails.Add("mostafa.gholamitousi@gmail.com");
            emails.Add("amirhossein.gholamitousi@gmail.com");

            foreach (var item in emails)
            {
                if (totalCount % 100 == 0)
                {
                    LogHelper.InfoLog($"EmailController|Send|Send Emails => Until now email count: {totalCount}, sent: {totalSendCount}, failed: {totalFailCount}");
                }

                var emailStatus = await MailHelper.SendEmail(email.Subject, email.EmailContent, @"amirhossein.gholamitousi@gmail.com", item);

                if (emailStatus)
                {
                    totalSendCount++;
                }
                else
                {
                    totalFailCount++;
                }

                totalCount++;
                Thread.Sleep(1250);
            }

            email.IsSend = true;
            email.SendDate = DateTime.Now;

            var status = await _emailTemplate.Update(email);
            var date = DateTime.Now.Subtract(startDate);

            LogHelper.InfoLog($"EmailController|Send||Send Emails => Total Count: {totalCount}, Date Time: {date.Hours}:{date.Minutes}:{date.Seconds}");
            return status > 0;
            #endregion
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("EmailController|Send", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion
}