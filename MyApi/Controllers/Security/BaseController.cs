using System.Net;
using Common.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace MyApi.Controllers.Common
{
    [Authorize]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseController : ControllerBase
    {
        protected string GetIp()
        {
            return GetClientIp();
        }
        private string GetClientIp()
        {
            try
            {
                var ipAddress = HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
                var ipv4Addresses = Array.FindAll(Dns.GetHostEntry(ipAddress).AddressList, a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                return ipv4Addresses.Any() ? ipv4Addresses.Last().ToString() : null;
            }
            catch
            {
                return null;
            }
        }


        public long UserId => User.GetUserId() ?? 0;
    }
}