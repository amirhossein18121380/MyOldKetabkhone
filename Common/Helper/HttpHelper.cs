using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Common.Helper
{
    public static class HttpHelper
    {
        public static ContentResult ExceptionContent(Exception ex) 
        {
            //LogHelper.ErrorLog("در عملیات درخواستی شما خطایی رخ داده");
    
            return new ContentResult
            {
                Content = "در عملیات درخواستی شما خطایی رخ داده",
                StatusCode = (int)HttpStatusCode.InternalServerError,
                ContentType = "text/html"
            };
        }

        public static ContentResult AccessDeniedContent()
        {
            LogHelper.InfoLog("شما به این سرویس دسترسی ندارید");

            return new ContentResult
            {
                Content = "شما به این سرویس دسترسی ندارید",
                StatusCode = (int)HttpStatusCode.BadRequest,
                ContentType = "text/html"
            };
        }

        public static ContentResult InvalidContent()
        {
            LogHelper.InfoLog("اطلاعات ارسالی به سرور معتبر نمی باشند");

            return new ContentResult
            {
                Content = "اطلاعات ارسالی به سرور معتبر نمی باشند",
                StatusCode = (int)HttpStatusCode.BadRequest,
                ContentType = "text/html"
            };
        }

        public static ContentResult FailedContent(string msgContent)
        {
            LogHelper.InfoLog(msgContent);
            return new ContentResult
            {
                Content = msgContent,
                StatusCode = (int)HttpStatusCode.BadRequest,
                ContentType = "text/html"
            };
        }

        public static ContentResult NotFoundContent(string contentMessage)
        {
            return new ContentResult
            {
                Content = contentMessage,
                StatusCode = (int)HttpStatusCode.NotFound,
                ContentType = "text/html"
            };
        }
    }
}
