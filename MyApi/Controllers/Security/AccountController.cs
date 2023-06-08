#region Using
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DataModel.ViewModel.Security.Account;
using DataModel.ViewModel.Security.User;
using DataModel.Models;
using Services.Helpers;
using DataModel.ViewModel.Security;
using Common.Helper;
using DataAccess.DAL;
using MyApi.Controllers.Common;
using DataAccess.Interface;
using DataAccess.Interface.Security;
using System.Text.RegularExpressions;
using MyApi.Helpers;
using Common.Enum;
using DataAccess.Interface.Transaction;
using DataModel.Models.Transaction;
using DataModel.Models.Security;
#endregion

namespace MyApi.Controllers.Security;

[Route("api/[controller]")]
[ApiController]
public class AccountController : BaseController
{
    #region Constructor
    private readonly IUser _user;
    private readonly IWalletDal _wallet;
    private readonly ITokenFactoryService _tokenFactoryService;
    private readonly ITokenStoreService _tokenStoreService;
    private readonly IInviteHistoryDal _inviteHistory;
    private readonly IRolePolicyDal _rolePolicy;

    public AccountController(IUser user, ITokenFactoryService tokenFactoryService, ITokenStoreService tokenStoreService,
        IWalletDal wallet, IInviteHistoryDal inviteHistory, IRolePolicyDal rolePolicy)
    {
        _user = user;
        _tokenFactoryService = tokenFactoryService;
        _tokenStoreService = tokenStoreService;
        _wallet = wallet;
        _inviteHistory = inviteHistory;
        _rolePolicy = rolePolicy;
    }

    #endregion

    #region Register
    [HttpPost]
    [Route("Register")]
    [AllowAnonymous]
    //[IgnoreAntiforgeryToken]
    public async Task<ActionResult<LoginResultViewModel>> Register(RegisterViewModel data)
    {
        try
        {

            #region Validation
            if (!ModelState.IsValid || string.IsNullOrEmpty(data.CaptchaCode) || string.IsNullOrEmpty(data.MobileNumber?.Trim()) ||
            string.IsNullOrEmpty(data.Email?.Trim()) || string.IsNullOrEmpty(data.UserName?.Trim()) || string.IsNullOrEmpty(data.Password))
            {
                return BadRequest("The Sending Info Is Not Valid.");
            }

            //var regex = new Regex(@"^[A-Za-z0-9]{1,}[._-]?[A-Za-z0-9]{3,}$");
            var regex = new Regex(@"^(?!.*\.\.)(?!.*\.$)[^\W][\w.]{3,29}$");
            if (!regex.IsMatch(data.UserName.Trim()))
            {
                return BadRequest("Invalid UserName");
            }

            //if (!data.NationalCode.Trim().IsValidNationalCode())
            //{
            //    return BadRequest("کد ملی نا معتبر می باشد");
            //}

            #region Captch

            //var isHuman = await new CaptchaVerificationHelper().IsCaptchaValid(data.CaptchaCode);

            //if (!isHuman)
            //{
            //    return HttpHelper.FailedContent("Picture code is not Valid");
            //}
            //*******************************************************************************************
            //var yourFirstCaptcha = new SimpleCaptcha();
            //bool isHuman = yourFirstCaptcha.Validate(data.CaptchaCode, data.CaptchaId);

            //if (!isHuman)
            //{
            //    return HttpHelper.FailedContent("Picture code is not Valid");
            //}
            #endregion

            var mobile = $"+{data.MobileNumber.Trim().Replace("+", string.Empty)}";
            var usrMob = await _user.GetByMobileNumber(mobile);

            if (usrMob != null)
            {
                return BadRequest("this mobilenumber is already Exists.");
            }

            var usrEmail = await _user.GetByEmail(data.Email.Trim());

            if (usrEmail != null)
            {
                return BadRequest("this Email is already Exists.");
            }

            //var usrNat = await _user.GetByNationalCode(data.NationalCode.Trim());

            //if (usrNat != null)
            //{
            //    return BadRequest("این کد ملی در سیستم ثبت شده است");
            //}

            var usr = await _user.GetByUserName(data.UserName.Trim());

            if (usr != null)
            {
                return BadRequest("This UserName is already Exist.");
            }

            //var siteSetting = await _availableData.GetListByDomain("SiteConfig");

            //var maxRegisterCountFromOneIp = 0;

            //if (siteSetting.Any())
            //{
            //    var maxRegCountFromIp = siteSetting.FirstOrDefault(c => c.DomainKey == "MaxRegisterCountFromOneIp");

            //    int.TryParse(maxRegCountFromIp?.DomainValue, out maxRegisterCountFromOneIp);
            //}

            //var ip = GetIp();

            //if (maxRegisterCountFromOneIp > 0 && ip != null)
            //{
            //    var ipCount = await _user.GetRegistrationIpCount(ip);

            //    if (ipCount >= maxRegisterCountFromOneIp)
            //    {
            //        return BadRequest("Your Ip Full");
            //    }
            //}
            #endregion

            #region Insert User
            var model = new User
            {
                UserName = data.UserName.Trim(),
                Password = SecurityHelper.AdvancePasswordHash(data.Password),
                MobileNumber = mobile,
                //CountryCode = data.CountryCode,
                //CountryIso = data.CountryIso,
                CreateOn = DateTime.Now,
                CreatorId = 1,
                Email = data.Email.Trim(),
                IsActive = true,
                IsBane = false,
                IsPanelUser = false,
                UniqCode = Guid.NewGuid().ToString(),
                ParentId = null,
                EmailVerified = false,
                MobileVerified = false,
                //RegistrationIp = ip?.Trim(),
                //NationalCode = data.NationalCode.Trim(),
                ChatStatus = (short)ChatStatusEnum.Disable
            };

            if (!string.IsNullOrEmpty(data.InviteCode))
            {
                model.ParentId = await GetParentIdFromInviteCode(data.InviteCode);
            }

            var userId = await _user.Insert(model);

            if (userId <= 0)
            {
                LogHelper.WarningLog("Can not insert user in db ==> AccountController|Register");
            }

            model.Id = userId;
            #endregion

            #region Insert User Wallet
            var walletId = await _wallet.Insert(new Wallet
            {
                WalletType = (short)WalletTypeEnum.MainWallet,
                EntityType = (short)WalletEntityTypeEnum.User,
                EntityId = userId,
                LastBalance = 0,
                CreateOn = DateTime.Now
            });

            if (walletId <= 0)
            {
                //await MongoLogging.WarningLogAsync("AccountController|Register", $"Can not insert wallet for user, userId: { userId}");
            }
            #endregion

            #region Insert Invite History
            if (model.ParentId.HasValue)
            {
                var inviteRecord = await _inviteHistory.GetHistoryRecord(model.Id);
                if (inviteRecord == null)
                {
                    await _inviteHistory.Insert(new InviteHistory
                    {
                        IsGetGift = false,
                        ParentUserId = model.ParentId.Value,
                        RegisterDate = model.CreateOn,
                        UserId = model.Id
                    });
                }
            }
            #endregion

            #region Create Response
            var result = await _tokenFactoryService.CreateJwtTokensAsync(model);
            await _tokenStoreService.AddUserTokenAsync(model, result.RefreshTokenSerial, result.AccessToken, null);
            //_antiForgery.RegenerateAntiForgeryCookies(result.Claims);

            //bool? mobileVerified = null;
            //var siteConfig = await _availableData.GetListByDomain("SiteConfig");
            //var verifyProp = siteConfig.FirstOrDefault(c => c.DomainKey == "VerifyByMobileNumber");

            //if (verifyProp != null && int.TryParse(verifyProp.DomainValue, out var val) && val == 1)
            //{
            //    mobileVerified = false;
            //}

            var retModel = new LoginResultViewModel
            {
                Token = result.AccessToken,
                RefreshToken = result.RefreshToken,
                ProfileInfo = new ProfileInfoViewModel
                {
                    UserId = model.Id,
                    UserName = model.UserName,
                    LastBalance = 0,
                    DisplayName = model.UserName,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    //CountryCode = model.CountryCode,
                    // CountryIso = model.CountryIso,
                    UnreadNotificationCount = 1,
                    InviteCode = "0",
                    EmailVerified = false,
                    //MobileVerified = mobileVerified
                }
            };

            return retModel;
            #endregion

        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("AccountController|Register", ex);
            return HttpHelper.FailedContent("There is a problem in regestering a user");
        }
    }

    private async Task<long?> GetParentIdFromInviteCode(string inviteCode)
    {
        if (string.IsNullOrEmpty(inviteCode.Trim()))
        {
            return null;
        }

        var user = await _user.GetByUniqCode(inviteCode.Trim());

        return user?.Id;
    }

    #endregion

    #region Login
    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    //[IgnoreAntiforgeryToken]
    public async Task<ActionResult<LoginResultViewModel>> Login(LoginViewModel model)
    {
        try
        {
            #region Validation

            if (string.IsNullOrEmpty(model.UserName.Trim()) || string.IsNullOrEmpty(model.Password) ||
                string.IsNullOrEmpty(model.CaptchaCode))
            {
                return HttpHelper.InvalidContent();
            }

            //var isHuman = await new CaptchaVerificationHelper().IsCaptchaValid(model.CaptchaCode);

            //if (!isHuman)
            //{
            //    return HttpHelper.FailedContent("سیستم شما را ربات تشخیص داده است. لطفا مجددا تلاش کنید");
            //}
            //********************************************************************************************************
            //var yourFirstCaptcha = new SimpleCaptcha();
            //bool isHuman = yourFirstCaptcha.Validate(model.CaptchaCode, model.CaptchaId);

            //if (!isHuman)
            //{
            //    var allowUserNames = new[]
            //    {
            //        "jesus", "amir"
            //    };
            //    if (!allowUserNames.Contains(model.UserName.ToLower()))
            //    {
            //        return HttpHelper.FailedContent("The picture code is not valid");
            //    }
            //}

            var passHash = SecurityHelper.AdvancePasswordHash(model.Password);
            var userModel = await _user.GetByUserNameAndPassword(model.UserName.Trim(), passHash);

            if (userModel == null)
            {
                userModel = await _user.GetByEmailAndPassword(model.UserName.Trim(), passHash);

                if (userModel == null)
                {
                    return HttpHelper.FailedContent("Username Or Password is not valid");
                }
            }

            if (!userModel.IsActive)
            {
                return HttpHelper.FailedContent("This User is not active");
            }

            if (userModel.IsBane)
            {
                return HttpHelper.FailedContent("The User is Bane");
            }
            #endregion

            userModel.LastLoggedIn = DateTime.Now;
            var updateStatus = await _user.Update(userModel);

            if (updateStatus <= 0)
            {
                LogHelper.WarningLog($"AccountController|Login => Problem in update user, userId: {userModel.Id}");
            }

            var result = await _tokenFactoryService.CreateJwtTokensAsync(userModel);
            await _tokenStoreService.AddUserTokenAsync(userModel, result.RefreshTokenSerial, result.AccessToken, null);
            //_antiForgery.RegenerateAntiForgeryCookies(result.Claims);

            var lastBalance = await _wallet.GetUserLastBalance(userModel.Id);
            //var messageCount = await _messages.GetUserUnreadMessageCount(userModel.Id);

            //bool? mobileVerified = null;
            //if (!userModel.MobileVerified)
            //{
            //    var siteConfig = await _availableData.GetListByDomain("SiteConfig");
            //    var verifyProp = siteConfig.FirstOrDefault(c => c.DomainKey == "VerifyByMobileNumber");

            //    if (verifyProp != null && int.TryParse(verifyProp.DomainValue, out var val) && val == 1)
            //    {
            //        mobileVerified = false;
            //    }
            //}
            //else
            //{
            //    mobileVerified = true;
            //}

            var retModel = new LoginResultViewModel
            {
                Token = result.AccessToken,
                RefreshToken = result.RefreshToken,
                ProfileInfo = new ProfileInfoViewModel
                {
                    ProfileImageName = userModel.ProfilePictureName,
                    LastBalance = lastBalance,
                    UserId = userModel.Id,
                    UserName = userModel.UserName,
                    DisplayName = !string.IsNullOrEmpty(userModel.DisplayName)
                        ? userModel.DisplayName
                        :
                        $"{userModel.FirstName} {userModel.LastName}".Trim().Length > 0
                            ?
                            $"{userModel.FirstName} {userModel.LastName}"
                            : userModel.UserName,
                    MobileNumber = userModel.MobileNumber,
                    //CountryCode = userModel.CountryCode,
                    //CountryIso = userModel.CountryIso,
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    Email = userModel.Email,
                    GenderType = userModel.GenderType,
                    BirthDay = userModel.BirthDay,
                    //UnreadNotificationCount = messageCount,
                    //InviteCode = userModel.EmailVerified && (!mobileVerified.HasValue || mobileVerified.Value) ? userModel.UniqCode : "0",
                    InviteCode = userModel.EmailVerified ? userModel.UniqCode : "0",
                    //MobileVerified = mobileVerified,
                    EmailVerified = userModel.EmailVerified,
                    //NationalCode = userModel.NationalCode
                }
            };

            return retModel;

        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("AccountController|Login", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }

    [HttpPost]
    [Route("PanelLogin")]
    [AllowAnonymous]
    //[IgnoreAntiforgeryToken]
    public async Task<ActionResult<PanelLoginResultViewModel>> PanelLogin(LoginViewModel model)
    {
        try
        {
            #region Validation

            //if (string.IsNullOrEmpty(model.UserName.Trim()) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.CaptchaCode))
            //{
            //    return HttpHelper.InvalidContent();
            //}

            //var yourFirstCaptcha = new SimpleCaptcha();
            //bool isHuman = yourFirstCaptcha.Validate(model.CaptchaCode, model.CaptchaId);

            //if (!isHuman)
            //{
            //    return HttpHelper.FailedContent("Incorrect captcha code");
            //}

            //var isHuman = await new CaptchaVerificationHelper().IsCaptchaValid(model.CaptchaCode);

            //if (!isHuman)
            //{
            //    return HttpHelper.FailedContent("Our system has detected you as a robot. Please try again.");
            //}

            var passHash = SecurityHelper.AdvancePasswordHash(model.Password);
            var userModel = await _user.GetByUserNameAndPassword(model.UserName.Trim(), passHash);

            if (userModel == null)
            {
                return HttpHelper.FailedContent("Incorrect user name or password");
            }

            if (!userModel.IsPanelUser || !userModel.IsActive || userModel.IsBane)
            {
                return HttpHelper.FailedContent("Access Denied");
            }

            var resourceIds = await _rolePolicy.GetResourceIdsByUserId(userModel.Id);

            if (!resourceIds.Any())
            {
                return HttpHelper.FailedContent("Access Denied");
            }
            #endregion

            userModel.LastLoggedIn = DateTime.Now;
            var updateStatus = await _user.Update(userModel);
            
            if (updateStatus <= 0)
            {
                LogHelper.WarningLog($"AccountController|PanelLogin ==> Problem in update user, userId: { userModel.Id}");
            }

            var result = await _tokenFactoryService.CreateJwtTokensAsync(userModel, string.Join(',', resourceIds));
            await _tokenStoreService.AddUserTokenAsync(userModel, result.RefreshTokenSerial, result.AccessToken, null);
            // _antiForgery.RegenerateAntiForgeryCookies(result.Claims);

            var retModel = new PanelLoginResultViewModel
            {
                Token = result.AccessToken,
                RefreshToken = result.RefreshToken,
                ResourceIds = resourceIds,
                ProfileInfo = new PanelInfoViewModel
                {
                    ProfileImageName = userModel.ProfilePictureName,
                    UserId = userModel.Id,
                    UserName = userModel.UserName,
                    DisplayName = !string.IsNullOrEmpty(userModel.DisplayName)
                        ? userModel.DisplayName
                        :
                        $"{userModel.FirstName} {userModel.LastName}".Trim().Length > 0
                            ?
                            $"{userModel.FirstName} {userModel.LastName}"
                            : userModel.UserName,
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                }
            };

            return retModel;
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("AccountController|PanelLogin", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region RefreshToken
    [AllowAnonymous]
    [HttpPost("RefreshToken")]
    //[IgnoreAntiforgeryToken]
    public async Task<ActionResult<RefreshTokenViewModel>> RefreshToken([FromBody] Token model)
    {
        try
        {
            var refreshTokenValue = model.RefreshToken;
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                return BadRequest("refreshToken is not set.");
            }

            var token = await _tokenStoreService.FindTokenAsync(refreshTokenValue);
            if (token == null)
            {
                return Unauthorized();
            }

            var user = await _user.GetById(token.UserId);

            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _tokenFactoryService.CreateJwtTokensAsync(user);
            await _tokenStoreService.AddUserTokenAsync(user, result.RefreshTokenSerial, result.AccessToken,
                _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue));

            //_antiForgery.RegenerateAntiForgeryCookies(result.Claims);

            return new RefreshTokenViewModel { Token = result.AccessToken, RefreshToken = result.RefreshToken };
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("AccountController|RefreshToken", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region Logout
    [HttpGet]
    [Route("Logout")]
    public async Task<ActionResult<bool>> Logout(string refreshToken)
    {
        try
        {
            #region Validation

            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            var user = await _user.GetById(UserId);

            if (user == null)
            {
                return HttpHelper.InvalidContent();
            }

            #endregion

            if (User.Identity is ClaimsIdentity claimsIdentity)
            {
                var userIdValue = claimsIdentity.FindFirst(ClaimTypes.UserData)?.Value;

                if (userIdValue == null)
                {
                    return HttpHelper.InvalidContent();
                }

                await _tokenStoreService.RevokeUserBearerTokensAsync(userIdValue, refreshToken);
            }

            var updateStatus = await _user.Update(user);

            if (updateStatus > 0)
            {
                //_antiForgery.DeleteAntiForgeryCookies();
                return true;
            }

            return HttpHelper.InvalidContent();
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("AccountController|Logout", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region Forgot Password
    [HttpPost]
    [Route("ForgetPassword")]
    [AllowAnonymous]
    //[IgnoreAntiforgeryToken]
    public async Task<ActionResult<string>> ForgetPassword(string userName)
    {
        try
        {
            #region Validation

            if (string.IsNullOrEmpty(userName.Trim()))
            {
                return HttpHelper.InvalidContent();
            }

            var userModel = await _user.GetByUserName(userName.Trim());

            if (userModel == null)
            {
                userModel = await _user.GetByEmail(userName.Trim());
                if (userModel == null)
                {
                    return HttpHelper.FailedContent("this is no account by this email or username");
                }
            }

            if (!userModel.IsActive)
            {
                return HttpHelper.FailedContent("your account is not active");
            }

            if (userModel.IsBane)
            {
                return HttpHelper.FailedContent("your account is bane");
            }

            #endregion

            var forgetToken = Guid.NewGuid().ToString();
            userModel.ForgetPasswordToken = forgetToken;
            userModel.ForgetPasswordTokenExpiration = DateTime.Now.AddMinutes(30);

            var updateStatus = await _user.Update(userModel);

            if (updateStatus <= 0)
            {
                LogHelper.InfoLog($"ForgetPassword => Problem in update user, userId: {userModel.Id}");
            }

            var name = $"{userModel.FirstName} {userModel.LastName}".Trim().Length > 0
                ? $"{userModel.FirstName} {userModel.LastName}".Trim()
                : userModel.UserName;

            var bodyText = $@"<p>{name} عزیز</p>
                                  <p>برای بازیابی کلمه عبور لطفا بر روی لینک زیر کلیک نمایید</p>
                                  <p>نام کاربری: {userModel.UserName}</p>
                                  <p><a href=""https://site.kipol.ir/resetPassword/{forgetToken}"">بازیابی کلمه عبور</a></p>
                    <p> با تشکر - تیم پشتیبانی پکر نت </p>";

            //var mailStatus = await MailHelper.SendEmail("YourSite - Your password recovery request!",
            //    bodyText, "pokernet.tech@gmail.com", userModel.Email);

            //if (!mailStatus)
            //{
            //    return HttpHelper.FailedContent("متاسفانه در این لحظه امکان ارسال ایمیل برای شما مقدور نمی باشد");
            //}

            var mailStatus = await MailHelper.SendEmail("ketabkhone - Your password recovery request!",
            bodyText, @"amirhossein.gholamitousi@gmail.com", userModel.Email);

            if (!mailStatus)
            {
                return HttpHelper.FailedContent("Can Not Send Email");
            }

            return "Check Your Mail For Reset Password";
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("AccountController|ForgetPassword", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region Reset Password
    [HttpPost]
    [Route("ResetPassword")]
    [AllowAnonymous]
    //[IgnoreAntiforgeryToken]
    public async Task<ActionResult<bool>> ResetPassword(ResetPasswordViewModel data)
    {
        try
        {
            #region Validation
            if (!ModelState.IsValid)
            {
                return BadRequest("اطلاعات ارسالی به سرویس معتبر نمی باشد");
            }

            var userModel = await _user.GetByForgetPasswordToken(data.ForgetToken);

            if (userModel == null)
            {
                return HttpHelper.FailedContent("اطلاعات ارسالی به سرویس معتبر نمی باشد");
            }

            if (!userModel.IsActive)
            {
                return HttpHelper.FailedContent("اکانت شما غیر فعال می باشد");
            }

            if (userModel.IsBane)
            {
                return HttpHelper.FailedContent("اکانت شما در لیست سیاه سایت می باشد و امکان استفاده از سایت را ندارد");
            }

            if (userModel.ForgetPasswordTokenExpiration < DateTime.Now)
            {
                return HttpHelper.FailedContent("فرصت بازیابی رمز عبور شما به اتمام رسیده است");
            }

            #endregion

            userModel.Password = SecurityHelper.AdvancePasswordHash(data.Password);
            userModel.ForgetPasswordTokenExpiration = null;
            userModel.ForgetPasswordToken = null;

            var status = await _user.Update(userModel);

            return status > 0;
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("AccountController|ResetPassword", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion
}