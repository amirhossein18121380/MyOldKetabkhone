#region Using
using Common.Helper;
using DataAccess.DAL;
using DataAccess.Interface;
using DataAccess.Interface.Security;
using DataModel.Models;
using DataModel.ViewModel.Security.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Helpers;

#endregion

namespace MyApi.Controllers.Security
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TokenController : ControllerBase
    {
        #region constructor
        private readonly string _secret;
        private readonly IUser _user;

        public TokenController(IConfiguration config, IUser user)
        {
            _secret = config.GetSection("JwtConfig").GetSection("secret").Value;
            _user = user;
        }
        #endregion

        [HttpPost]
        public async Task<ActionResult<string>> PostAsync([FromBody] LoginViewModel login)
        {
            var person = await IsValidUserAndPasswordCombinationAsync(login.UserName, login.Password);
            if (person != null)
            {
                return Ok(JwtHelper.GenerateToken(person.Id, _secret));
            }

            return NotFound();
        }

        private async Task<User> IsValidUserAndPasswordCombinationAsync(string username, string password)
        {
            var passHash = SecurityHelper.AdvancePasswordHash(password);
            var person = await _user.GetByUserNameAndPassword(username, passHash);

            return person;
        }
    }
}