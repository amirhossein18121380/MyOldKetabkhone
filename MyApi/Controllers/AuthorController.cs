#region Stuff
using System.Drawing;
using Common.Enum;
using Common.Extension;
using Common.Helper;
using DataAccess.DAL.Common;
using DataAccess.Interface;
using DataAccess.Tool;
using DataModel.Common;
using DataModel.Models;
using DataModel.Validator;
using DataModel.Validator.ResponseModel;
using DataModel.ViewModel;
using DataModel.ViewModel.Common;
using FluentValidation;
using FluentValidation.Results;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using MyApi.Controllers.Common;
using MyApi.Helpers;
using MyApi.Helpers.ValidationHelper;
using Newtonsoft.Json.Linq;

namespace MyApi.Controllers;
#endregion

[Route("api/[controller]")]
[ApiController]
public class AuthorController : BaseController
{
    #region Constructor
    private readonly IAuthorDal _author;
    private readonly IFileSystem _fileSystem;

    public AuthorController(IAuthorDal author, IFileSystem fileSystem)
    {
        _author = author;
        _fileSystem = fileSystem;
    }
    #endregion

    #region Fetch
    [HttpPost]
    [Route("GetList")]
    //[IgnoreAntiforgeryToken]
    [PkPermission(ResourcesEnum.AuthorManagement)]
    public async Task<ActionResult<PagedResponse<List<AuthorViewModel>>>> GetList(AuthorGetListFilterViewModel filterModel)
    {
        try
        {
            var users = await _author.GetList(filterModel);

            if (!users.data.Any())
            {
                return HttpHelper.InvalidContent();
            }

            var result = new PagedResponse<List<AuthorViewModel>>(users.data, users.totalCount);
            return result;
        }
        catch (Exception ex)
        {
            //await MongoLogging.ErrorLogAsync("UserController|GetList", ex.Message, ex.StackTrace);
            return HttpHelper.ExceptionContent(ex);
        }
    }

    [HttpGet]
    [Route("GetAllAuthors")]
    //[IgnoreAntiforgeryToken]
    [PkPermission(ResourcesEnum.AuthorManagement)]
    public async Task<ActionResult<List<Author>>> GetAllAuthors()
    {
        try
        {
            var author = await _author.GetAll();
            if (author == null) return HttpHelper.InvalidContent();
            return Ok(author);
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("authorController|GetAllAuthors", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }

    [HttpGet]
    [Route("GetAuthorProfileByAuthorId/{id}")]
    [PkPermission(ResourcesEnum.AuthorManagement)]
    public async Task<ActionResult<AuthorProfileViewModel>> GetAuthorProfileByAuthorId(long Id)
    {
        try
        {
            if (UserId <= 0)
            {
                HttpHelper.InvalidContent();
            }
            var author = await _author.GetAuthorProfileById(Id);
            if (author == null) return HttpHelper.InvalidContent();
            return Ok(author);
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("authorController|GetALlAuthorProfile", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }

    [HttpGet]
    [Route("GetAuthorProfileById/{authorid}")]
    [PkPermission(ResourcesEnum.AuthorManagement)]
    public async Task<ActionResult<AuthorProfileViewModel>> GetAuthorProfileById(long authorid)
    {
        try
        {
            if (UserId <= 0)
            {
                HttpHelper.InvalidContent();
            }
            var author = await _author.GetAuthorProfileById(authorid);
            if (author == null) return HttpHelper.InvalidContent();
            return Ok(author);
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("authorController|GetAuthorProfileById", ex);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("GetAuthorById/{id}")]
    [PkPermission(ResourcesEnum.AuthorManagement)]
    public async Task<ActionResult<AuthorViewModel>> GetAuthorById(long id)
    {
        try
        {
            if (UserId <= 0)
            {
                HttpHelper.InvalidContent();
            }
            var results = await _author.GetById(id);
            if (results == null) return NotFound("Not Found this author with this Id");

            var resultmodel = new AuthorViewModel()
            {
                //Id = results.Id,
                AuthorFirstName = results.AuthorFirstName,
                AuthorLastName = results.AuthorLastName,
                ProfilePictureName = results.ProfilePictureName,
                Birthday = results.Birthday,
                Country = results.Country,
                Language = results.Language,
                Bio = results.Bio,
                Age = results.Age,
            };
            return resultmodel;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetlightprofileById/{id}")]
    [PkPermission(ResourcesEnum.AuthorManagement)]
    public async Task<ActionResult<AuthorScore>> GetlightprofileById(long id)
    {
        try
        {
            if (UserId <= 0)
            {
                HttpHelper.InvalidContent();
            }
            var results = await _author.GetlightprofileById(id);
            if (results == null) return NotFound("Not Found this author with this Id");

            return results;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetLightprofile")]
    //[IgnoreAntiforgeryToken]
    [PkPermission(ResourcesEnum.AuthorManagement)]
    public async Task<ActionResult<List<AuthorScore>>> GetLightprofile()
    {
        try
        {
            var author = await _author.GetLightprofile();
            if (author == null) return HttpHelper.InvalidContent();
            return Ok(author);
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("authorController|GetAllAuthors", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }


    [HttpGet]
    [Route("BooksByAuthorId/{id}")]
    [PkPermission(ResourcesEnum.AuthorManagement)]
    public async Task<ActionResult<List<Book>>> BooksByAuthorId(long id)
    {
        try
        {
            if (UserId <= 0)
            {
                HttpHelper.InvalidContent();
            }
            var results = await _author.BooksByAuthorId(id);
            if (results == null) return NotFound("Not Found any book with this Id");
            return results;
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("AuthorController|BooksByAuthorId", ex);
            return Problem(ex.Message);
        }
    }

    #endregion

    #region Insert

    [HttpPost]
    [Route("AddAuthor")]
    [PkPermission(ResourcesEnum.AuthorManagement)]
    public async Task<ActionResult<bool>> AddAuthor()
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                HttpHelper.InvalidContent();
            }

            //AuthorViewModel authorVm = new AuthorViewModel();

            //var validation = AuthorValidation.ApplyAuthorValidator(authorVm);
            //if (validation.ValidationMessages.Count > 0 & validation.ValidationMessages != null)
            //{
            //    return BadRequest(validation.ValidationMessages);
            //}

            //var authoremail = await _author.GetByEmail(authorVm.Email?.Trim());
            //if (authoremail != null)
            //{
            //    //LogHelper.ErrorLog("This Email is already Token");
            //    return BadRequest("This Email is already Token");
            //}

            //var name = $"{author.AuthorFirstName} {author.AuthorLastName}".Trim().Length > 0
            //        ? $"{author.AuthorFirstName} {author.AuthorLastName}".Trim()
            //        : author.AuthorFirstName + author.AuthorLastName;
            //***********************

            //var authorcheck = await _author.GetByFirstnameAndLastname(authorVm.AuthorFirstName, authorVm.AuthorLastName);
            //if (authorcheck != null)
            //{
            //    //LogHelper.ErrorLog("This author first and last name is already taken");
            //    return BadRequest("This author first and last name is already taken");
            //}

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
            var data = json.ToObject<UpdateAuthorInfoViewModel>();

            if (data == null)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region insert
            var author = new Author
            {
                AuthorFirstName = data.AuthorFirstName,
                AuthorLastName = data.AuthorLastName,
                //ProfilePictureId = authorVm.ProfilePictureId,
                //ProfilePictureName = authorVm.ProfilePictureName,
                Birthday = data.Birthday.ToLocalTime(),             
                Country = data.Country,
                Language = data.Language,
                //Age = Math.Abs(DateTime.Now.Year - data.Birthday.Year),
                Age = data.Age,
                Bio = data.Bio,
            };

            var userId = await _author.Insert(author);

            author.Id = userId;
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

                var fileName = $"PI_{author.Id}_{DateTime.Now.Ticks}";
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
                    author.ProfilePictureId = pictureImageId;
                    author.ProfilePictureName = fileName;

                    var conti = await _author.Update(author);
                }
            }
            #endregion

            if (userId <= 0)
            {
                return HttpHelper.FailedContent("Isn't possible to record a user in this moment.");
            }

            //author.Id = userId;

            return userId > 0;
        }
        catch (Exception ex)
        {
            //return Problem(ex.Message);
            //await LogHelper.ErrorLogAsync("authorController|addauthor", ex.Message, ex.StackTrace);
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

            var user = await _author.GetById(userId);
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
            LogHelper.ErrorLog("AuthorController|GetProfilePicture", ex);
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

            var user = await _author.GetById(userId);
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
            LogHelper.ErrorLog("AuthorController|GetThumbnailProfilePicture", ex);
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
            LogHelper.ErrorLog("AuthorController|GetThumbnailProfilePictureByName", ex);
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

    #region UploadImage
    [HttpPost]
    [Route("SetProfileInfo")]
    [IgnoreAntiforgeryToken]
    [PkPermission(ResourcesEnum.AuthorManagement)]
    public async Task<ActionResult<bool>> SetAuthorProfileInfo()
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.InvalidContent();
            }

            var userModel = await _author.GetById(UserId);

            if (userModel == null || userModel.Id <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

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
            var data = json.ToObject<AuthorViewModel>();

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

            #region author Profile Data
            userModel.Birthday = data.Birthday;
            userModel.AuthorFirstName = data.AuthorFirstName?.Trim();
            userModel.AuthorLastName = data.AuthorLastName?.Trim();
            userModel.Country = data.Country;
            userModel.Language = data.Language!.Trim();
            userModel.Age = data.Age;
            userModel.Bio = data.Bio?.Trim();

            var updateStatus = await _author.Update(userModel);

            if (deletedImageId.HasValue)
            {
                await _fileSystem.Delete(deletedImageId.Value);
            }

            return updateStatus > 0;
            #endregion
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("AuthorController|SetProfileInfo", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }

    [HttpPost]
    [Route("SetProfileImage")]
    [RegisterUpload]
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
            var userModel = await _author.GetById(UserId);

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
            var updateState = await _author.Update(userModel);

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
            LogHelper.ErrorLog("AuthorController|UploadImage|SetProfileImage", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }


    [HttpPost]
    [Route("UploadImage")]
    //[RegisterUpload]
    //[IgnoreAntiforgeryToken]
    public async Task<ActionResult<bool>> UploadImage([FromForm(Name = "userImage")] IFormFile img)
    {
        try
        {
            #region Validation

            if (UserId <= 0)
            {
                HttpHelper.InvalidContent();
            }
            var userModel = await _author.GetById(UserId);

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
                return HttpHelper.FailedContent("خطا در ذخیره سازی تصویر");
            }

            userModel.ProfilePictureId = pictureImageId;
            userModel.ProfilePictureName = fileName;
            var updateState = await _author.Update(userModel);

            if (updateState <= 0)
            {
                return HttpHelper.FailedContent("خطا در ذخیره سازی تصویر");
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
            return HttpHelper.ExceptionContent(ex);
        }
    }

    #endregion

    #region DeleteImage
    [HttpPost]
    [Route("DeleteImage")]
    //[IgnoreAntiforgeryToken]
    [PkPermission(ResourcesEnum.AuthorManagement)]

    public async Task<ActionResult<bool>> DeleteImage(long userId)
    {
        try
        {
            #region Validation

            if (userId <= 0)
            {
                HttpHelper.InvalidContent();
            }

            var userModel = await _author.GetById(userId);

            if (userModel == null || userModel.Id <= 0)
            {
                return HttpHelper.InvalidContent();
            }

            #endregion

            if (!userModel.ProfilePictureId.HasValue)
                return true;
            var profilePictureId = userModel.ProfilePictureId.Value;
            userModel.ProfilePictureId = null;
            userModel.ProfilePictureName = null;
            var updateStatus = await _author.Update(userModel);

            if (updateStatus >= 0)
            {
                await _fileSystem.Delete(profilePictureId);
            }

            return true;
        }
        catch (Exception ex)
        {
            //await MongoLogging.ErrorLogAsync("UserController|DeleteImage", ex.Message, ex.StackTrace);

            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region Update

    [HttpPost]
    [Route("UpdateAuthor")]
    [PkPermission(ResourcesEnum.AuthorManagement)]
    public async Task<ActionResult<bool>> UpdateAuthor()
    {
        try
        {
            #region ModelValidation
            if (UserId <= 0)
            {
                HttpHelper.InvalidContent();
            }

            //AuthorViewModel authorVm = new AuthorViewModel();

            //var validation = AuthorValidation.ApplyAuthorValidator(authorVm);
            //if (validation.ValidationMessages.Count > 0 & validation.ValidationMessages != null)
            //{
            //    return BadRequest(validation.ValidationMessages);
            //}
            #endregion

            #region UniqValidation
            //var authoremail = await _author.GetByEmail(authorVm.Email?.Trim());
            //if (authoremail != null)
            //{
            //    //LogHelper.ErrorLog("This Email is already Token");
            //    return BadRequest("This Email is already Token");
            //}

            //var name = $"{author.AuthorFirstName} {author.AuthorLastName}".Trim().Length > 0
            //        ? $"{author.AuthorFirstName} {author.AuthorLastName}".Trim()
            //        : author.AuthorFirstName + author.AuthorLastName;


            //*************************************************************
            //var authorcheck = await _author.GetByFirstnameAndLastname(authorVm.AuthorFirstName, authorVm.AuthorLastName);
            //if (authorcheck != null)
            //{
            //    //LogHelper.ErrorLog("This author first and last name is already taken");
            //    return BadRequest("This author first and last name is already taken");
            //}

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
            var data = json.ToObject<UpdateAuthorInfoViewModel>();

            if (data == null)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion


            if (data.AuthorId <= 0)
            {
                return HttpHelper.InvalidContent();
            }

            var userModel = await _author.GetById(data.AuthorId);

            if (userModel == null || userModel.Id <= 0)
            {
                return HttpHelper.InvalidContent();
            }

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

            //Id = authorVm.Id,
            userModel.AuthorFirstName = data.AuthorFirstName;
            userModel.AuthorLastName = data.AuthorLastName;
            userModel.Birthday = data.Birthday;
            userModel.Country = data.Country;
            userModel.Language = data.Language;
            //userModel.Age = Math.Abs(DateTime.Now.Year - data.Birthday.Year);
            userModel.Age =  data.Age;
            userModel.Bio = data.Bio;
            
            //var author = new Author
            //{
            //    //Id = authorVm.Id,
            //    AuthorFirstName = data.AuthorFirstName,
            //    AuthorLastName = data.AuthorLastName,
            //    Birthday = data.Birthday,
            //    Country = data.Country,
            //    Language = data.Language,
            //    Age = Math.Abs(DateTime.Now.Year - data.Birthday.Year),
            //    Bio = data.Bio,
            //};

            var result = await _author.Update(userModel);

            if (result <= 0)
            {
                return HttpHelper.FailedContent("Isn't possible to Update a user in this moment.");
            }

            if (deletedImageId.HasValue)
            {
                await _fileSystem.Delete(deletedImageId.Value);
            }
            //author.Id = result;

            return result > 0;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
            //return StatusCode(500, ex.Message);
        }
    }

    #endregion

    #region Delete

    [HttpGet]
    [Route("DeleteAuthor/{Id}")]
    [PkPermission(ResourcesEnum.AuthorManagement)]
    public async Task<ActionResult<bool>> DeleteAuthor(long Id)
    {
        try
        {
            if (UserId <= 0)
            {
                HttpHelper.InvalidContent();
            }
            var result = await _author.Delete(Id);
            return result;
        }
        catch (Exception ex)
        {
            return HttpHelper.ExceptionContent(ex);
            //return Problem(ex.Message);
        }
    }
    #endregion
}