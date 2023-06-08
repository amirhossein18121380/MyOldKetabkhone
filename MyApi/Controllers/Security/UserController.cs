#region Using
using System.Drawing;
using Common.Enum;
using Common.Extension;
using Common.Helper;
using DataAccess.DAL.Common;
using DataAccess.Interface.Security;
using DataAccess.Interface.Transaction;
using DataModel.Common;
using DataModel.Models;
using DataModel.Models.Transaction;
using DataModel.ViewModel.Common;
using DataModel.ViewModel.Role;
using DataModel.ViewModel.Security.Account;
using DataModel.ViewModel.Security.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using MyApi.Controllers.Common;
using MyApi.Helpers;
using Newtonsoft.Json.Linq;
using PokerNet.DataModel.ViewModel.Security;

#endregion

namespace MyApi.Controllers.Security
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        #region Constructor
        private readonly IUser _user;
        private readonly IRoleMemberDal _roleMember;
        private readonly IRoleDal _role;
        private readonly IRolePolicyDal _rolePolicy;
        private readonly IFileSystem _fileSystem;
        private readonly IWalletDal _wallet;
        private readonly IInviteHistoryDal _inviteHistory;
        public UserController(IUser user, IRoleMemberDal roleMember, IRoleDal role, IRolePolicyDal rolePolicy, 
            IFileSystem fileSystem, IWalletDal wallet, IInviteHistoryDal inviteHistory)
        {
            _user = user;
            _roleMember = roleMember;
            _role = role;
            _rolePolicy = rolePolicy;
            _fileSystem = fileSystem;
            _wallet = wallet;
            _inviteHistory = inviteHistory;
        }
        #endregion

        #region GetList
        [HttpPost]
        [Route("GetList")]
        //[IgnoreAntiforgeryToken]
        [PkPermission(ResourcesEnum.UserManagement)]
        public async Task<ActionResult<PagedResponse<List<UserGetListViewModel>>>> GetList(UserGetListFilterViewModel filterModel)
        {
            try
            {
                var users = await _user.GetList(filterModel);

                if (!users.data.Any())
                {
                    return HttpHelper.InvalidContent();
                }

                var result = new PagedResponse<List<UserGetListViewModel>>(users.data, users.totalCount);
                return result;
            }
            catch (Exception ex)
            {
                //await MongoLogging.ErrorLogAsync("UserController|GetList", ex.Message, ex.StackTrace);
                return HttpHelper.ExceptionContent(ex);
            }
        }


        [HttpGet]
        [Route("GetPanelProfileInfo")]
        public async Task<ActionResult<PanelInfoViewModel>> GetPanelProfileInfo()
        {
            try
            {
                #region Validation
                if (UserId <= 0)
                {
                    return HttpHelper.AccessDeniedContent();
                }

                var userModel = await _user.GetById(UserId);

                if (userModel == null || userModel.Id <= 0)
                {
                    return HttpHelper.InvalidContent();
                }

                if (!userModel.IsPanelUser || !userModel.IsActive || userModel.IsBane)
                {
                    return HttpHelper.AccessDeniedContent();
                }

                var resourceIds = await _rolePolicy.GetResourceIdsByUserId(userModel.Id);

                if (!resourceIds.Any())
                {
                    return HttpHelper.FailedContent("Access Denied");
                }

                #endregion

                #region Create Response
           

                var retModel = new PanelInfoViewModel
                {
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    UserName = userModel.UserName,
                    DisplayName = userModel.DisplayName,
                    ProfileImageName = userModel.ProfilePictureName,
                    UserId = userModel.Id,
                    //UnseenWithdrawalCount = unseenWithdrawalCount,
                    //UnseenSuggestionCount = unseenSuggestionCount
                };

                return retModel;
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|GetPanelProfileInfo", ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }
        #endregion

        #region GetById
        [HttpGet]
        [Route("GetById/{id}")]
        [PkPermission(ResourcesEnum.UserManagement)]
        public async Task<ActionResult<UserViewModel>> GetById(long id)
        {
            try
            {
                if (id <= 0)
                {
                    HttpHelper.InvalidContent();
                }

                var userModel = await _user.GetById(id);

                if (userModel == null)
                {
                    return HttpHelper.InvalidContent();
                }

                var roles = await _role.GetList();
                var roleMember = await _roleMember.GetByUserId(userModel.Id);

                var resultModel = new UserViewModel
                {
                    UserId = userModel.Id,
                    UserName = userModel.UserName,
                    //CountryIso = userModel.CountryIso,
                    //CountryCode = userModel.CountryCode,
                    DisplayName = userModel.DisplayName,
                    ProfilePictureName = userModel.ProfilePictureName,
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    Email = userModel.Email,
                    MobileNumber = userModel.MobileNumber,
                    IsPanelUser = userModel.IsPanelUser,
                    IsActive = userModel.IsActive,
                    GenderType = userModel.GenderType,
                    IsBane = userModel.IsBane,
                    ChatStatus = userModel.ChatStatus,
                    BirthDay = userModel.BirthDay,
                    //NationalCode = userModel.NationalCode,
                    Roles = roles.Select(c => new AccessibleRoleViewModel
                    {
                        RoleId = c.Id,
                        RoleName = c.Title,
                        IsAccess = roleMember.Any(rm => rm.RoleId == c.Id)
                    }).ToList()
                };

                return resultModel;
            }
            catch (Exception ex)
            {
                //await Logging.ErrorLogAsync("UserController|GetById", ex.Message, ex.StackTrace);
                return HttpHelper.ExceptionContent(ex);
            }
        }
        #endregion

        #region get some info
        private async Task<ActionResult<ProfileInfoViewModel>> GetUserProfile(long userId)
        {
            var userModel = await _user.GetById(userId);

            if (userModel == null || userModel.Id <= 0)
            {
                return HttpHelper.InvalidContent();
            }

            var lastBalance = await _wallet.GetUserLastBalance(userModel.Id);
        
            //var unreadMessageCount = await _messages.GetUserUnreadMessageCount(userId);

            var retModel = new ProfileInfoViewModel
            {
                BirthDay = userModel.BirthDay,
                Email = userModel.Email,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                GenderType = userModel.GenderType,
                MobileNumber = userModel.MobileNumber,
                //CountryCode = userModel.CountryCode,
                //CountryIso = userModel.CountryIso,
                UserName = userModel.UserName,
                DisplayName = userModel.DisplayName,
                LastBalance = lastBalance,
                ProfileImageName = userModel.ProfilePictureName,
                UserId = userModel.Id,
                //Level = userModel.UserLevel,                
                //UnreadNotificationCount = unreadMessageCount,
                //InviteCode = userModel.EmailVerified && (!mobileVerified.HasValue || mobileVerified.Value) ? userModel.UniqCode : "0",
                //MobileVerified = mobileVerified,
                EmailVerified = userModel.EmailVerified,
                NationalCode = userModel.NationalCode
            };

            return retModel;
        }

        [HttpGet]
        [Route("ProfileInfo")]
        public async Task<ActionResult<ProfileInfoViewModel>> GetProfileInfo()
        {
            try
            {
                if (UserId <= 0)
                {
                    return HttpHelper.InvalidContent();
                }

                var retModel = await GetUserProfile(UserId);

                return retModel;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|GetProfileInfo", ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }

        [HttpPost]
        [Route("SetProfileOtherProfileInfo")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<ProfileInfoViewModel>> SetProfileOtherProfileInfo(ProfileInfoViewModel data)
        {
            try
            {
                #region Validation
                if (UserId <= 0)
                {
                    return HttpHelper.AccessDeniedContent();
                }

                var userModel = await _user.GetById(UserId);

                if (userModel == null || userModel.Id <= 0)
                {
                    return HttpHelper.AccessDeniedContent();
                }

                if (string.IsNullOrEmpty(userModel.NationalCode) && !string.IsNullOrEmpty(data.NationalCode?.Trim()))
                {
                    if (!data.NationalCode.Trim().IsValidNationalCode())
                    {
                        return HttpHelper.FailedContent("Invalid NationalCode");
                    }
                }
                #endregion

                #region User Profile Data
                userModel.BirthDay = data.BirthDay;
                userModel.FirstName = data.FirstName?.Trim();
                userModel.LastName = data.LastName?.Trim();
                userModel.GenderType = data.GenderType;
                userModel.DisplayName = data.DisplayName?.Trim();


                //if (string.IsNullOrEmpty(userModel.NationalCode))
                //{
                //    userModel.NationalCode = data.NationalCode?.Trim();
                //}

                var updateStatus = await _user.Update(userModel);

                if (updateStatus <= 0)
                {
                    return HttpHelper.FailedContent("Error In Save Record In Db");
                }

                var retModel = await GetUserProfile(userModel.Id);
                return retModel;
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|SetProfileData", ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }

        #endregion

        #region UploadImage
        [HttpPost]
        [Route("SetProfileInfo")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<bool>> SetProfileInfo()
        {
            try
            {
                #region Validation
                if (UserId <= 0)
                {
                    return HttpHelper.InvalidContent();
                }

                var userModel = await _user.GetById(UserId);

                if (userModel == null || userModel.Id <= 0)
                {
                    return HttpHelper.InvalidContent();
                }
                #endregion

                #region Get Data From Multipart Request

                var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
                if (syncIOFeature != null)
                {
                    syncIOFeature.AllowSynchronousIO = true;
                }
                var multipart = await MultipartRequestHelper.GetMultiPart(Request, HttpContext);
                if (multipart == null)
                {
                    return HttpHelper.InvalidContent();
                }

                var textContent = multipart.TextContents;

                if (string.IsNullOrEmpty(textContent))
                {
                    return HttpHelper.InvalidContent();
                }

                var json = JObject.Parse(textContent);
                var data = json.ToObject<ProfileInfoViewModel>();

                if (data == null)
                {
                    return HttpHelper.InvalidContent();
                }
                #endregion

                #region Profile Picture
                var deletedImageId = (long?)null;

                if (multipart.ImageArrays != null)
                {
                    Image newImage;
                    await using var stream = new MemoryStream(multipart.ImageArrays);

                    using (var img = Image.FromStream(stream))
                    {
                        newImage = ImageHelper.ResizeImage(img, new Size(128, 128));
                    }

                    var thumbnailImage = newImage.ImageToByte();

                    var fileName = $"PI_{userModel.Id}_{DateTime.Now.Ticks}";
                    var fileSystemModel = new FileSystem
                    {
                        CreateOn = DateTime.Now,
                        CreatorId = 1,
                        FileType = multipart.FileExtension ?? ".jpg",
                        IsCompress = false,
                        FileData = multipart.ImageArrays,
                        FileName = fileName,
                        FileSize = multipart.ImageArrays.Length,
                        ThumbnailFileData = thumbnailImage,
                        ThumbnailFileName = $"Thumbnail_{fileName}",
                        ThumbnailFileSize = thumbnailImage.Length
                    };

                    if (userModel.ProfilePictureId.HasValue)
                    {
                        deletedImageId = userModel.ProfilePictureId.Value;
                    }

                    var pictureImageId = await _fileSystem.Insert(fileSystemModel);
                    if (pictureImageId > 0)
                    {
                        userModel.ProfilePictureId = pictureImageId;
                        userModel.ProfilePictureName = fileName;
                    }
                }
                else
                {
                    if (userModel.ProfilePictureId.HasValue)
                    {
                        deletedImageId = userModel.ProfilePictureId.Value;
                    }
                }
                #endregion

                #region User Profile Data
                userModel.BirthDay = data.BirthDay;
                userModel.Email = data.Email!.Trim();
                userModel.FirstName = data.FirstName?.Trim().ApplyUnifiedYeKe();
                userModel.LastName = data.LastName?.Trim().ApplyUnifiedYeKe();
                userModel.GenderType = data.GenderType;
                userModel.MobileNumber = data.MobileNumber!.Trim();
                userModel.UserName = data.UserName.Trim();
                userModel.DisplayName = data.DisplayName?.Trim().ApplyUnifiedYeKe();

                var updateStatus = await _user.Update(userModel);

                if (deletedImageId.HasValue)
                {
                    await _fileSystem.Delete(deletedImageId.Value);
                }

                return updateStatus > 0;
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|SetProfileInfo", ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }

        [HttpPost]
        [Route("SetProfileImage")]
        //[RegisterUpload]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<string>> SetProfileImage([FromForm(Name = "userImage")] IFormFile img)
        {
            try
            {
                #region Validation

                if (UserId <= 0)
                {
                    return HttpHelper.InvalidContent();
                }
                var userModel = await _user.GetById(UserId);

                if (userModel == null || userModel.Id <= 0)
                {
                    return HttpHelper.InvalidContent();
                }

                #endregion

                #region Save Picture                

                var deletedImageId = (long?)null;
                if (userModel.ProfilePictureId.HasValue)
                {
                    deletedImageId = userModel.ProfilePictureId.Value;
                }

                var imageArrays = await img.ToByteArray();
                var fileExtension = img.FileName.GetFileExtension();
                var fileName = $"PI_{UserId}_{DateTime.Now.Ticks}";

                Image newImage;
                await using var stream = new MemoryStream();
                await img.CopyToAsync(stream);

                using (var orgImg = Image.FromStream(stream))
                {
                    newImage = ImageHelper.ResizeImage(orgImg, new Size(128, 128));
                }

                var thumbnailImage = newImage.ImageToByte();

                var fileSystemModel = new FileSystem
                {
                    CreateOn = DateTime.Now,
                    CreatorId = 1,
                    FileType = fileExtension,
                    IsCompress = false,
                    FileData = imageArrays,
                    FileName = fileName,
                    FileSize = imageArrays.Length,
                    ThumbnailFileData = thumbnailImage,
                    ThumbnailFileName = $"Thumbnail_{fileName}",
                    ThumbnailFileSize = thumbnailImage.Length
                };

                var pictureImageId = await _fileSystem.Insert(fileSystemModel);

                if (pictureImageId <= 0)
                {
                    return HttpHelper.FailedContent("Error In Save Picture");
                }

                userModel.ProfilePictureId = pictureImageId;
                userModel.ProfilePictureName = fileName;
                var updateState = await _user.Update(userModel);

                if (updateState <= 0)
                {
                    return HttpHelper.FailedContent("Error In Save Picture");
                }

                if (deletedImageId.HasValue)
                {
                    await _fileSystem.Delete(deletedImageId.Value);
                }

                return fileName;
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|uploadimage|SetProfileImage", ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }


        [HttpPost]
        [Route("UploadImage")]
        //[RegisterUpload]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<bool>> UploadImage([FromForm(Name = "userImage")] IFormFile img)
        {
            try
            {
                #region Validation

                if (UserId <= 0)
                {
                    HttpHelper.InvalidContent();
                }
                var userModel = await _user.GetById(UserId);

                if (userModel == null || userModel.Id <= 0)
                {
                    return HttpHelper.InvalidContent();
                }

                #endregion

                #region Save Picture                

                var deletedImageId = (long?)null;
                if (userModel.ProfilePictureId.HasValue)
                {
                    deletedImageId = userModel.ProfilePictureId.Value;
                }

                var imageArrays = await img.ToByteArray();
                var fileExtension = Path.GetExtension(img.FileName);
                var fileName = $"PI_{UserId}_{DateTime.Now.Ticks}";

                Image newImage;
                await using var stream = new MemoryStream();
                await img.CopyToAsync(stream);

                using (var orgImg = Image.FromStream(stream))
                {
                    newImage = ImageHelper.ResizeImage(orgImg, new Size(128, 128));
                }

                var thumbnailImage = newImage.ToByteArray();

                var fileSystemModel = new FileSystem
                {
                    CreateOn = DateTime.Now,
                    CreatorId = 1,
                    FileType = fileExtension,
                    IsCompress = false,
                    FileData = imageArrays,
                    FileName = fileName,
                    FileSize = imageArrays.Length,
                    ThumbnailFileData = thumbnailImage,
                    ThumbnailFileName = $"Thumbnail_{fileName}",
                    ThumbnailFileSize = thumbnailImage.Length
                };

                var pictureImageId = await _fileSystem.Insert(fileSystemModel);

                if (pictureImageId <= 0)
                {
                    return HttpHelper.FailedContent("Error in save picture");
                }

                userModel.ProfilePictureId = pictureImageId;
                userModel.ProfilePictureName = fileName;
                var updateState = await _user.Update(userModel);

                if (updateState <= 0)
                {
                    return HttpHelper.FailedContent("Error in save picture");
                }

                if (deletedImageId.HasValue)
                {
                    await _fileSystem.Delete(deletedImageId.Value);
                }

                return true;
                #endregion
            }
            catch (Exception ex)
            {
                //await MongoLogging.ErrorLogAsync("UserController|UploadImage", ex.Message, ex.StackTrace);
                LogHelper.ErrorLog("UserController|UploadImage", ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }

        #endregion

        #region DeleteImage
        [HttpPost]
        [Route("DeleteImage")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<bool>> DeleteImage(long userId)
        {
            try
            {
                #region validation
                if (userId <= 0)
                {
                    HttpHelper.InvalidContent();
                }

                var userModel = await _user.GetById(userId);
                if(userModel == null || userModel.Id <= 0)
                {
                    return HttpHelper.InvalidContent();
                }
                #endregion

                if (!userModel.ProfilePictureId.HasValue)
                {
                    return true;
                }
                var profilePictureId = userModel.ProfilePictureId.Value;
                userModel.ProfilePictureId = null;
                userModel.ProfilePictureName = null;
                var updateStatus = await _user.Update(userModel);

                if(updateStatus >= 0)
                {
                    await _fileSystem.Delete(profilePictureId);
                }

                return true;
            }
            catch(Exception ex)
            {
                LogHelper.ErrorLog("UserController|DeleteImage", ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }
        #endregion

        #region GetProfilePicture
        [HttpGet]
        [Route("GetProfilePicture/{userId}")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GetProfilePicture(long userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return NotFound();
                }

                var user = await _user.GetById(userId);
                if (user == null)
                    return NotFound();

                if (user.ProfilePictureId.HasValue)
                {
                    var fileModel = await _fileSystem.GetById(user.ProfilePictureId.Value);

                    if (fileModel == null)
                    {
                        return NoContent();
                    }

                    return File(fileModel.FileData, "application/octet-stream", $"{fileModel.FileName}{fileModel.FileType}");
                }

                return NoContent();

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|GetProfilePicture", ex);
                return NoContent();
            }
        }

        [HttpGet]
        [Route("GetThumbnailProfilePicture/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetThumbnailProfilePicture(long userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return NotFound();
                }

                var user = await _user.GetById(userId);
                if (user == null)
                    return NotFound();

                if (user.ProfilePictureId.HasValue)
                {
                    var thumbnailFileModel = await _fileSystem.GetById(user.ProfilePictureId.Value);

                    if (thumbnailFileModel == null)
                    {
                        return NoContent();
                    }

                    return File(thumbnailFileModel.ThumbnailFileData, "application/octet-stream", $"{thumbnailFileModel.ThumbnailFileName}{thumbnailFileModel.FileType}");
                }

                return NoContent();

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|GetThumbnailProfilePicture", ex);
                return NotFound();
            }
        }

        [HttpGet]
        [Route("GetThumbnailProfilePictureByName/{fileName}")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GetThumbnailProfilePictureByName(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName.Trim()))
                {
                    return NotFound();
                }

                var fileModel = await _fileSystem.GetByThumbnailFileName($"Thumbnail_{fileName.Trim()}");

                if (fileModel == null)
                {
                    return NoContent();
                }

                return File(fileModel.ThumbnailFileData, "application/octet-stream", $"{fileModel.ThumbnailFileName}{fileModel.FileType}");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|GetThumbnailProfilePictureByName", ex);
                return NoContent();
            }
        }

        [HttpGet]
        [Route("GetProfilePictureByName/{fileName}")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GetProfilePictureByName(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName.Trim()))
                {
                    return NotFound();
                }

                var fileModel = await _fileSystem.GetByFileName(fileName.Trim());

                if (fileModel == null)
                {
                    return NoContent();
                }

                return File(fileModel.FileData, "application/octet-stream", $"{fileModel.FileName}{fileModel.FileType}");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|GetProfilePictureByName", ex);
                return NoContent();
            }
        }
        #endregion

        #region ChangePassword
        [HttpPost]
        [Route("ChangePassword")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<bool>> ChangePassword(ChangePasswordViewModel data)
        {
            try
            {
                #region Validation
                if (UserId <= 0)
                {
                    return HttpHelper.InvalidContent();
                }

                var userModel = await _user.GetById(UserId);

                if (userModel == null || userModel.Id <= 0)
                {
                    return HttpHelper.InvalidContent();
                }

                if (userModel.Password != SecurityHelper.AdvancePasswordHash(data.CurrentPassword))
                {
                    return HttpHelper.FailedContent("Your Current Password is incorrect");
                }
                #endregion

                #region User Profile Data
                userModel.Password = SecurityHelper.AdvancePasswordHash(data.NewPassword);
                var updateStatus = await _user.Update(userModel);
                return updateStatus > 0;
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|ChangePassword", ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }
        #endregion

        #region AdminChangePassword
        [HttpPost]
        [Route("AdminChangePassword")]
        [IgnoreAntiforgeryToken]
        [PkPermission(ResourcesEnum.UserManagement)]
        public async Task<ActionResult<bool>> AdminChangePassword(long userId, string newPassword)
        {
            try
            {
                #region Validation
                if (userId <= 0)
                {
                    return HttpHelper.InvalidContent();
                }

                var userModel = await _user.GetById(userId);

                if (userModel == null || userModel.Id <= 0)
                {
                    return HttpHelper.InvalidContent();
                }
                #endregion

                #region User Profile Data
                userModel.Password = SecurityHelper.AdvancePasswordHash(newPassword);
                var updateStatus = await _user.Update(userModel);
                return updateStatus > 0;
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|AdminChangePassword", ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }
        #endregion

        #region Add User
        [HttpPost]
        [Route("AddUser")]
        [IgnoreAntiforgeryToken]
        [PkPermission(ResourcesEnum.UserManagement)]
        public async Task<ActionResult<bool>> AddUser()
        {
            try
            {
                #region Get Data From Multipart Request

                var multipart = await MultipartRequestHelper.GetMultiPart(Request, HttpContext);
                if (multipart == null)
                {
                    return HttpHelper.InvalidContent();
                }

                var textContent = multipart.TextContents;

                if (string.IsNullOrEmpty(textContent))
                {
                    return HttpHelper.InvalidContent();
                }

                var json = JObject.Parse(textContent);
                var data = json.ToObject<UpdateUserInfoViewModel>();

                if (data == null)
                {
                    return HttpHelper.InvalidContent();
                }
                #endregion

                #region Validation

                if (UserId <= 0)
                {
                    return HttpHelper.AccessDeniedContent();
                }

                if (string.IsNullOrEmpty(data.UserName.Trim()) || string.IsNullOrEmpty(data.Email.Trim()) ||
                    string.IsNullOrEmpty(data.MobileNumber.Trim()) || string.IsNullOrEmpty(data.CountryCode.Trim()) ||
                    string.IsNullOrEmpty(data.CountryIso.Trim()))
                {
                    return HttpHelper.InvalidContent();
                }

                if (!string.IsNullOrEmpty(data.NationalCode?.Trim()))
                {
                    if (!data.NationalCode.Trim().IsValidNationalCode())
                    {
                        return BadRequest();
                    }

                    var usrNat = await _user.GetByNationalCode(data.NationalCode.Trim());

                    if (usrNat != null)
                    {
                        return BadRequest();
                    }
                }

                var mobile = $"+{data.MobileNumber.Trim().Replace("+", string.Empty)}";
                var usrMob = await _user.GetByMobileNumber(mobile);

                if (usrMob != null)
                {
                    return BadRequest();
                }

                var checkUserName = await _user.GetByUserName(data.UserName.Trim());
                if (checkUserName != null)
                {
                    return HttpHelper.FailedContent("This username is already exist");
                }

                var checkEmail = await _user.GetByEmail(data.Email.Trim());
                if (checkEmail != null)
                {
                    return HttpHelper.FailedContent("This email is already exist");
                }
                #endregion

                #region Insert User Data
                var userModel = new User
                {
                    BirthDay = data.BirthDay,
                    Email = data.Email.Trim(),
                    FirstName = data.FirstName?.Trim(),
                    LastName = data.LastName?.Trim(),
                    GenderType = data.GenderType,
                    MobileNumber = mobile,
                    CountryCode = data.CountryCode.Trim(),
                    CountryIso = data.CountryIso.Trim(),
                    UserName = data.UserName.Trim(),
                    DisplayName = data.DisplayName?.Trim(),
                    IsPanelUser = data.IsPanelUser,
                    CreateOn = DateTime.Now,
                    CreatorId = UserId,
                    IsActive = data.IsActive,
                    IsBane = data.IsBane,
                    ChatStatus = data.ChatStatus,
                    Password = SecurityHelper.AdvancePasswordHash(data.Password),
                    UniqCode = StringHelper.GenarateTrCode(),
                    NationalCode = data.NationalCode?.Trim()
                };

                var userId = await _user.Insert(userModel);

                if (userId <= 0)
                {
                    return HttpHelper.InvalidContent();
                }

                userModel.Id = userId;
                #endregion

                #region Insert Role Member
                foreach (var roleId in data.RoleIds)
                {
                    await _roleMember.Insert(new RoleMember
                    {
                        UserId = userModel.Id,
                        RoleId = roleId,
                        CreateOn = DateTime.Now,
                        CreatorId = UserId
                    });
                }
                #endregion

                #region Profile Picture
                if (multipart.ImageArrays != null)
                {
                    Image newImage;
                    await using var stream = new MemoryStream(multipart.ImageArrays);

                    using (var img = Image.FromStream(stream))
                    {
                        newImage = ImageHelper.ResizeImage(img, new Size(128, 128));
                    }

                    var thumbnailImage = newImage.ImageToByte();

                    var fileName = $"PI_{userModel.Id}_{DateTime.Now.Ticks}";
                    var fileSystemModel = new FileSystem
                    {
                        CreateOn = DateTime.Now,
                        CreatorId = 1,
                        FileType = multipart.FileExtension ?? ".jpg",
                        IsCompress = false,
                        FileData = multipart.ImageArrays,
                        FileName = fileName,
                        FileSize = multipart.ImageArrays.Length,
                        ThumbnailFileData = thumbnailImage,
                        ThumbnailFileName = $"Thumbnail_{fileName}",
                        ThumbnailFileSize = thumbnailImage.Length
                    };

                    var pictureImageId = await _fileSystem.Insert(fileSystemModel);
                    if (pictureImageId > 0)
                    {
                        userModel.ProfilePictureId = pictureImageId;
                        userModel.ProfilePictureName = fileName;

                        await _user.Update(userModel);
                    }
                }
                #endregion

                #region Insert Wallet

                await _wallet.Insert(new Wallet
                {
                    WalletType = (short)WalletTypeEnum.MainWallet,
                    EntityType = (short)WalletEntityTypeEnum.User,
                    EntityId = userId,
                    LastBalance = 0,
                    CreateOn = DateTime.Now
                });
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|AddUser",ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }
        #endregion

        #region Edit User
        [HttpPost]
        [Route("UpdateUserInfo")]
        [IgnoreAntiforgeryToken]
        [PkPermission(ResourcesEnum.UserManagement)]
        public async Task<ActionResult<bool>> UpdateUserInfo()
        {
            try
            {
                #region Get Data From Multipart Request
                var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
                if (syncIOFeature != null)
                {
                    syncIOFeature.AllowSynchronousIO = true;
                }
                var multipart = await MultipartRequestHelper.GetMultiPart(Request, HttpContext);
                if (multipart == null)
                {
                    return HttpHelper.InvalidContent();
                }

                var textContent = multipart.TextContents;

                if (string.IsNullOrEmpty(textContent))
                {
                    return HttpHelper.InvalidContent();
                }

                var json = JObject.Parse(textContent);
                var data = json.ToObject<UpdateUserInfoViewModel>();

                if (data == null)
                {
                    return HttpHelper.InvalidContent();
                }
                #endregion

                #region Validation
                if (data.UserId <= 0)
                {
                    return HttpHelper.InvalidContent();
                }

                var userModel = await _user.GetById(data.UserId);

                if (userModel == null || userModel.Id <= 0)
                {
                    return HttpHelper.InvalidContent();
                }

                if (!string.IsNullOrEmpty(data.NationalCode?.Trim()))
                {
                    if (!data.NationalCode.Trim().IsValidNationalCode())
                    {
                        return HttpHelper.FailedContent("something wrong with national code");
                    }

                    if (userModel.NationalCode?.Trim() != data.NationalCode.Trim())
                    {
                        var usrNat = await _user.GetByNationalCode(data.NationalCode.Trim());

                        if (usrNat != null)
                        {
                            return BadRequest();
                        }
                    }
                }
                #endregion

                #region Profile Picture
                var deletedImageId = (long?)null;

                if (multipart.ImageArrays != null)
                {
                    Image newImage;
                    await using var stream = new MemoryStream(multipart.ImageArrays);

                    using (var img = Image.FromStream(stream))
                    {
                        newImage = ImageHelper.ResizeImage(img, new Size(128, 128));
                    }

                    var thumbnailImage = newImage.ImageToByte();

                    var fileName = $"PI_{userModel.Id}_{DateTime.Now.Ticks}";
                    var fileSystemModel = new FileSystem
                    {
                        CreateOn = DateTime.Now,
                        CreatorId = 1,
                        FileType = multipart.FileExtension ?? ".jpg",
                        IsCompress = false,
                        FileData = multipart.ImageArrays,
                        FileName = fileName,
                        FileSize = multipart.ImageArrays.Length,
                        ThumbnailFileData = thumbnailImage,
                        ThumbnailFileName = $"Thumbnail_{fileName}",
                        ThumbnailFileSize = thumbnailImage.Length
                    };

                    if (userModel.ProfilePictureId.HasValue)
                    {
                        deletedImageId = userModel.ProfilePictureId.Value;
                    }

                    var pictureImageId = await _fileSystem.Insert(fileSystemModel);
                    if (pictureImageId > 0)
                    {
                        userModel.ProfilePictureId = pictureImageId;
                        userModel.ProfilePictureName = fileName;
                    }
                }
                #endregion

                #region User Profile Data
                userModel.BirthDay = data.BirthDay;
                userModel.Email = data.Email.Trim();
                userModel.FirstName = data.FirstName?.Trim();
                userModel.LastName = data.LastName?.Trim();
                userModel.GenderType = data.GenderType;
                userModel.MobileNumber = data.MobileNumber.Trim();
                userModel.CountryCode = data.CountryCode.Trim();
                userModel.CountryIso = data.CountryIso.Trim();
                userModel.UserName = data.UserName.Trim();
                userModel.DisplayName = data.DisplayName?.Trim();
                userModel.IsPanelUser = data.IsPanelUser;
                userModel.IsActive = data.IsActive;
                userModel.IsBane = data.IsBane;
                userModel.ChatStatus = data.ChatStatus;
                userModel.NationalCode = data.NationalCode?.Trim();

                await _user.Update(userModel);

                if (deletedImageId.HasValue)
                {
                    await _fileSystem.Delete(deletedImageId.Value);
                }
                #endregion

                #region Update Role Member
                await _roleMember.DeleteUserRole(userModel.Id);

                foreach (var roleId in data.RoleIds)
                {
                    await _roleMember.Insert(new RoleMember
                    {
                        UserId = userModel.Id,
                        RoleId = roleId,
                        CreateOn = DateTime.Now,
                        CreatorId = UserId
                    });
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|UpdateUserInfo", ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }
        #endregion

        #region Verify User
        [HttpPost]
        [Route("UserVerificationRequest")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<UserVerificationRequestResultViewModel>> UserVerificationRequest(UserVerificationRequestViewModel data)
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
                    return HttpHelper.AccessDeniedContent();
                }

                if (data.VerifyType <= 0 || data.VerifyType > 2 || data.VerifyValue == null || string.IsNullOrEmpty(data.VerifyValue.Trim()))
                {
                    return HttpHelper.InvalidContent();
                }
                #endregion

                #region Email Verification
                if (data.VerifyType == 1)
                {
                    #region Validation New Email
                    if (user.EmailVerified)
                    {
                        return new UserVerificationRequestResultViewModel
                        {
                            UserId = UserId,
                            Status = (short)VerifyStatusEnum.ValueApproved,
                            Message = "This Email is already verified"
                        };
                    }

                    var email = data.VerifyValue.Trim();

                    if (user.Email != email)
                    {
                        var userEmailModel = await _user.GetByEmail(email);
                        if (userEmailModel != null)
                        {
                            return new UserVerificationRequestResultViewModel
                            {
                                UserId = UserId,
                                Status = (short)VerifyStatusEnum.NewEmailInUse,
                                Message = "This email is in used"
                            };
                        }
                    }
                    #endregion

                    #region Update Data Base Record
                    var code = StringHelper.GenarateRandomNumber(6);
                    user.Email = email;
                    user.EmailVerifyCode = code;
                    await _user.Update(user);
                    #endregion

                    #region Send Email

                    //var emailDomainList = await _availableData.GetListByDomain("EmailValidation");

                    //if (emailDomainList.Any())
                    //{
                    //    var emailHosts = emailDomainList.Where(c => c.DomainKey == "AllowDomainName")
                    //        .Select(c => c.DomainValue.ToLower()).ToArray();

                    //    if (emailHosts.Any())
                    //    {
                    //        var add = new System.Net.Mail.MailAddress(email);
                    //        var domain = add.Host;

                    //        if (emailHosts.All(c => c != domain.ToLower()))
                    //        {
                    //            return new UserVerificationRequestResultViewModel
                    //            {
                    //                UserId = UserId,
                    //                Status = (short)VerifyStatusEnum.IsOk
                    //            };
                    //        }
                    //    }

                    //}

                    var name = $"{user.FirstName} {user.LastName}".Trim().Length > 0
                        ? $"{user.FirstName} {user.LastName}".Trim()
                        : user.UserName;

                    var bodyText = $@"<p>{name} عزیز</p>
                                          <p>کد اعتبار سنجی ایمیل شما: {code}</p>
                                          <p>با تشکر - پوکر نت</p>";

                    var mailStatus = await MailHelper.SendEmail("PokerNet - Email verification code!",
                        bodyText, @"info@pokernet.vip", email);

                    if (!mailStatus)
                    {
                        return new UserVerificationRequestResultViewModel
                        {
                            UserId = UserId,
                            Status = (short)VerifyStatusEnum.ProblemInSendCode,
                            Message = "در ارسال ایمیل مشکلی پیش آمده است"
                        };
                    }

                    return new UserVerificationRequestResultViewModel
                    {
                        UserId = UserId,
                        Status = (short)VerifyStatusEnum.IsOk
                    };
                    #endregion
                }
                #endregion

                #region Mobile Verification

                //#region Validation New Mobile Number
                //if (user.MobileVerified)
                //{
                //    return new UserVerificationRequestResultViewModel
                //    {
                //        UserId = UserId,
                //        Status = (short)VerifyStatusEnum.ValueApproved,
                //        Message = _localizer["MobileNumber Already Verified"].Value
                //    };
                //}

                //var siteConfig = await _availableData.GetListByDomain("SiteConfig");
                //var verifyProp = siteConfig.FirstOrDefault(c => c.DomainKey == "VerifyByMobileNumber");

                //if (verifyProp != null && int.TryParse(verifyProp.DomainValue, out var val) && val != 1)
                //{
                //    return new UserVerificationRequestResultViewModel
                //    {
                //        UserId = UserId,
                //        Status = (short)VerifyStatusEnum.OperationNotAllowed,
                //        Message = _localizer["Mobile Verification Not Allow This Time"].Value
                //    };
                //}

                //var mobile = $"+{data.VerifyValue.Trim().Replace("+", string.Empty)}";

                //if (user.MobileNumber != mobile)
                //{
                //    var usrMob = await _user.GetByMobileNumber(mobile);
                //    if (usrMob != null)
                //    {
                //        return new UserVerificationRequestResultViewModel
                //        {
                //            UserId = UserId,
                //            Status = (short)VerifyStatusEnum.NeWMobileInUse,
                //            Message = _localizer["Duplicate MobileNumber"].Value
                //        };
                //    }
                //}
                //#endregion

                //#region Update Data Base Record

                //if (user.MobileVerifyCode != null)
                //{
                //    if (user.MobileVerifyCode == "1" && user.MobileVerifiedDate.HasValue && user.MobileVerifiedDate.Value >= DateTime.Now)
                //    {
                //        var min = user.MobileVerifiedDate.Value.Subtract(DateTime.Now).Minutes;
                //        var sec = user.MobileVerifiedDate.Value.Subtract(DateTime.Now).Seconds;

                //        return new UserVerificationRequestResultViewModel
                //        {
                //            UserId = UserId,
                //            Status = (short)VerifyStatusEnum.ValueApproved,
                //            Message = _localizer["Mobile Verification Last Chance", min, sec].Value
                //        };
                //    }

                //    if (user.MobileVerifyCode == "2")
                //    {
                //        return new UserVerificationRequestResultViewModel
                //        {
                //            UserId = UserId,
                //            Status = (short)VerifyStatusEnum.ValueApproved,
                //            Message = _localizer["Mobile Verification End Chance"].Value
                //        };
                //    }
                //}
                //#endregion

                //#region Mis Call
                //var res = await MobileVerificationHelper.Verification(mobile);

                //if (!res)
                //{
                //    return new UserVerificationRequestResultViewModel
                //    {
                //        UserId = UserId,
                //        Status = (short)VerifyStatusEnum.ValueApproved,
                //        Message = _localizer["Problem In Mis Call"].Value
                //    };
                //}

                //user.MobileNumber = mobile;
                //user.MobileVerifyCode = user.MobileVerifyCode == null ? "1" : "2";
                //user.MobileVerifiedDate = DateTime.Now.AddMinutes(30);
                //await _user.Update(user);            
                #endregion
              
                return new UserVerificationRequestResultViewModel
                {
                    UserId = UserId,
                    Status = (short)VerifyStatusEnum.IsOk
                };
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|UserVerificationRequest", ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }

        [HttpPost]
        [Route("UserVerificationByCode")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<UserVerificationRequestResultViewModel>> UserVerificationByCode(UserVerificationViewModel data)
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
                    return HttpHelper.AccessDeniedContent();
                }

                if (data.VerifyType <= 0 || data.VerifyType > 2 || data.Code == null || string.IsNullOrEmpty(data.Code.Trim()))
                {
                    return HttpHelper.InvalidContent();
                }
                #endregion

                #region Email Verification
                if (data.VerifyType == 1)
                {
                    if (user.EmailVerified)
                    {
                        return new UserVerificationRequestResultViewModel
                        {
                            UserId = UserId,
                            Status = (short)VerifyStatusEnum.ValueApproved,
                            Message = "This Email is Already Verified"
                        };
                    }

                    if (user.EmailVerifyCode != data.Code)
                    {
                        user.EmailVerifyCode = null;
                        await _user.Update(user);

                        return new UserVerificationRequestResultViewModel
                        {
                            UserId = UserId,
                            Status = (short)VerifyStatusEnum.WrongCode,
                            Message = "Incorrect Code for email verification"
                        };
                    }

                    user.EmailVerifiedDate = DateTime.Now;
                    user.EmailVerifyCode = null;
                    user.EmailVerified = true;
                    await _user.Update(user);

                    await CalculateGift(user);

                    return new UserVerificationRequestResultViewModel
                    {
                        UserId = UserId,
                        Status = (short)VerifyStatusEnum.IsOk
                    };
                }
                #endregion

                #region Mobile Verification
                //if (user.MobileVerified)
                //{
                //    return new UserVerificationRequestResultViewModel
                //    {
                //        UserId = UserId,
                //        Status = (short)VerifyStatusEnum.ValueApproved,
                //        Message = "MobileNumber Already Verified"
                //    };
                //}

                //var providerNumber = data.Code.Trim().Replace("+1", string.Empty);

                //if (providerNumber.Length < 10)
                //{
                //    return new UserVerificationRequestResultViewModel
                //    {
                //        UserId = UserId,
                //        Status = (short)VerifyStatusEnum.ValueApproved,
                //        Message = _localizer["Incorrect Provider Number"].Value
                //    };
                //}

                //providerNumber = providerNumber.Substring(providerNumber.Length - 10);

                //var res = await MobileVerificationHelper.VerificationReport(user.MobileNumber, providerNumber);

                //if (!string.IsNullOrEmpty(res.ErrorCode) || res.Status != "SUCCESSFUL")
                //{
                //    return new UserVerificationRequestResultViewModel
                //    {
                //        UserId = UserId,
                //        Status = (short)VerifyStatusEnum.WrongCode,
                //        Message = _localizer["Incorrect Data"].Value
                //    };
                //}

                //user.MobileVerifiedDate = DateTime.Now;
                //user.MobileVerified = true;
                //user.MobileVerifyCode = null;
                //await _user.Update(user);

                //await CalculateGift(user);


                #endregion

                return new UserVerificationRequestResultViewModel
                {
                    UserId = UserId,
                    Status = (short)VerifyStatusEnum.IsOk
                };
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|UserVerificationByCode", ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }

        private async Task CalculateGift(User user)
        {
            try
            {
                #region Complete Validation
                //var activeValidationByMobile = false;
                //var siteConfig = await _availableData.GetListByDomain("SiteConfig");
                //var verifyProp = siteConfig.FirstOrDefault(c => c.DomainKey == "VerifyByMobileNumber");

                //if (verifyProp != null && int.TryParse(verifyProp.DomainValue, out var val) && val == 1)
                //{
                //    activeValidationByMobile = true;
                //}

                //if (activeValidationByMobile && !user.MobileVerified)
                //{
                //    return;
                //}
                //if (!user.EmailVerified)
                //{
                //    return;
                //}
                //#endregion

                //#region Check Get Registeration Gift
                //var isGetRegisterGift = await _transactionInfo.IsGetRegistrationGift(UserId);
                //if (isGetRegisterGift)
                //{
                //    return;
                //}
                #endregion

                #region Check If Promotion Actived
                //var promotionSettings = await _availableData.GetListByDomain("PromotionSetting");

                //if (promotionSettings.Any(c => c.DomainKey == "RegistrationGift"))
                //{
                //    var prom = promotionSettings.First(c => c.DomainKey == "RegistrationGift");
                //    long.TryParse(prom.DomainValue, out var promotionAmount);

                //    if (promotionAmount > 0)
                //    {
                //        var lastBalance = await _transactionInfo.IncreaseWalletAsync(user.Id, promotionAmount,
                //             PaymentMethodEnum.RegistrationGift, user.Id, "Gift for registration");

                //        var messageModel = new Messages
                //        {
                //            UserId = user.Id,
                //            CreateOn = DateTime.Now,
                //            CreatorId = 1,
                //            IsDeleted = false,
                //            IsRead = false,
                //            Subject = prom.Title,
                //            MessageContent = prom.Comment
                //        };

                //        var messageId = await _messages.Insert(messageModel);

                //        #region Send Notification From SignalR

                //        var unreadMessageCount = await _messages.GetUserUnreadMessageCount(UserId);
                //        var notification = new NotificationViewModel
                //        {
                //            Id = messageId,
                //            Subject = messageModel.Subject,
                //            UnreadCount = unreadMessageCount
                //        };

                //        await _gameHub.Clients.Group(GroupHelper.GetUserGroupName(messageModel.UserId))
                //            .Notification(notification);

                //        await _gameHub.Clients.Group(GroupHelper.GetUserGroupName(messageModel.UserId))
                //            .UpdateBalance(lastBalance);
                //        #endregion
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|CalculateGift", ex);
            }
        }
        #endregion

        #region Invite History InviteHistory
        [HttpPost]
        [Route("GetUserInviteList")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<PagedResponse<List<InviteHistoryViewModel>>>> GetUserInviteList(InviteHistoryFilterViewModel filterModel)
        {
            try
            {
                var result = await _inviteHistory.GetListByFilter(filterModel);

                if (result.data == null)
                {
                    return HttpHelper.InvalidContent();
                }

                return new PagedResponse<List<InviteHistoryViewModel>>(result.data, result.totalCount);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UserController|GetUserInviteList", ex);
                return HttpHelper.ExceptionContent(ex);
            }
        }
        #endregion
    }
}