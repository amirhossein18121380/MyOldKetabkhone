#region stuff
using System.Drawing;
using Common.Enum;
using Common.Extension;
using Common.Helper;
using DataAccess.DAL.Common;
using DataAccess.Interface;
using DataModel.Common;
using DataModel.Models;
using DataModel.ViewModel;
using DataModel.ViewModel.Common;
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
public class TranslatorController : BaseController
{
    #region Constructor
    private readonly ITranslatorDal _translator;
    private readonly IFileSystem _fileSystem;

    public TranslatorController(
        ITranslatorDal translator,
        IFileSystem fileSystem)
    {
        _translator = translator;
        _fileSystem = fileSystem;
    }
    #endregion

    #region Fetch

    [HttpPost]
    [Route("GetList")]
    //[IgnoreAntiforgeryToken]
    //[PkPermission(ResourcesEnum.TranslatorManagement)]
    public async Task<ActionResult<PagedResponse<List<TranslatorGetListViewModel>>>> GetList(TranslatorGetListFilterViewModel filterModel)
    {
        try
        {
            var users = await _translator.GetList(filterModel);

            if (!users.data.Any())
            {
                return HttpHelper.InvalidContent();
            }

            var result = new PagedResponse<List<TranslatorGetListViewModel>>(users.data, users.totalCount);
            return result;
        }
        catch (Exception ex)
        {
            //await MongoLogging.ErrorLogAsync("UserController|GetList", ex.Message, ex.StackTrace);
            LogHelper.ErrorLog("TranslatorController|GetList", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }

    [HttpGet]
    [Route("GetAllTranslators")]
    public async Task<ActionResult<List<Translator>>> GetAllTranslators()
    {
        try
        {
            var translator = await _translator.GetAll();
            if (translator == null) return HttpHelper.InvalidContent();
            return Ok(translator);
        }
        catch (Exception ex)
        {
            //log error
            return HttpHelper.ExceptionContent(ex);
            //return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("GetTranslatorById/{id}")]
    public async Task<ActionResult<Translator>> GetTranslatorById(long id)
    {
        try
        {
            var results = await _translator.GetById(id);
            if (results == null) return NotFound("Not Found this translator with this Id");
            return Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    #endregion

    #region Insert

    [HttpPost]
    [Route("AddTranslator")]
    public async Task<ActionResult<bool>> AddTranslator()
    {
        try
        {
            #region Validation

            //var validation = TranslatorValidation.ApplyTranslatorValidator(translatorvm);
            //if (validation.ValidationMessages.Count > 0 & validation.ValidationMessages != null)
            //{
            //    return BadRequest(validation.ValidationMessages);
            //}

            //var name = $"{author.AuthorFirstName} {author.AuthorLastName}".Trim().Length > 0
            //        ? $"{author.AuthorFirstName} {author.AuthorLastName}".Trim()
            //        : author.AuthorFirstName + author.AuthorLastName;

            //var authorcheck = await _translator.GetByFirstnameAndLastname(translatorvm.TranslatorFirstName,translatorvm.TranslatorLastName);
            //if (authorcheck != null)
            //{
            //    LogHelper.ErrorLog("This translator first and last name is already taken");
            //    return BadRequest("This translator first and last name is already taken");
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
            var data = json.ToObject<UpdateTranslatorInfoViewModel>();

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

            if (string.IsNullOrEmpty(data.TranslatorFirstName?.Trim()) || string.IsNullOrEmpty(data.TranslatorLastName?.Trim()) ||
                string.IsNullOrEmpty(data.Country?.Trim()) || string.IsNullOrEmpty(data.Language?.Trim()))
            {
                return HttpHelper.InvalidContent();
            }

            #endregion

            var translator = new Translator
            {
                TranslatorFirstName = data.TranslatorFirstName,
                TranslatorLastName = data.TranslatorLastName,
                Birthday = data.Birthday,
                Country = data.Country,
                Language = data.Language,
                Age = Math.Abs(DateTime.Now.Year - data.Birthday.Year),
                Bio = data.Bio,
            };

            var translatorId = await _translator.Insert(translator);

            if (translatorId <= 0)
            {
                return HttpHelper.FailedContent("Isn't possible to record a translator in this moment.");
            }

            translator.Id = translatorId;

            #region Profile Picture

            if (multipart.ImageArrays != null)
            {
                Image newImage;
                await using var stream = new MemoryStream(multipart.ImageArrays);

                 #pragma warning disable CA1416 // Validate platform compatibility
                using (var img = Image.FromStream(stream))
                 #pragma warning restore CA1416 // Validate platform compatibility
                {
                    newImage = ImageHelper.ResizeImage(img, new Size(128, 128));
                }

                var thumbnailImage = newImage.ImageToByte();

                var fileName = $"PI_{translator.Id}_{DateTime.Now.Ticks}";
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
                    translator.ProfilePictureId = pictureImageId;
                    translator.ProfilePictureName = fileName;

                    await _translator.Update(translator);
                }
            }
            #endregion

            return true;
        }

        catch (Exception ex)
        {
            return Problem(ex.Message);
        }

    }
    #endregion

    #region Update

    [HttpPost]
    [Route("UpdateTranslator")]
    public async Task<ActionResult<bool>> UpdateTranslator()
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
            var data = json.ToObject<UpdateTranslatorInfoViewModel>();

            if (data == null)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Validation
            //if (data.UserId <= 0)
            //{
            //    return HttpHelper.InvalidContent();
            //}

            var userModel = await _translator.GetById(data.TranslatorId);

            if (userModel == null || userModel.Id <= 0)
            {
                return HttpHelper.InvalidContent();
            }

            //var validation = TranslatorValidation.ApplyTranslatorValidator(translatorvm);
            //if (validation.ValidationMessages.Count > 0 & validation.ValidationMessages != null)
            //{
            //    return BadRequest(validation.ValidationMessages);
            //}

            //var name = $"{author.AuthorFirstName} {author.AuthorLastName}".Trim().Length > 0
            //        ? $"{author.AuthorFirstName} {author.AuthorLastName}".Trim()
            //        : author.AuthorFirstName + author.AuthorLastName;

            //var authorcheck = await _translator.GetByFirstnameAndLastname(data.TranslatorFirstName, data.TranslatorLastName);
            //if (authorcheck != null)
            //{
            //    return BadRequest("This translator first and last name is already taken");
            //}


            #endregion

            #region Profile Picture
            var deletedImageId = (long?)null;

            if (multipart.ImageArrays != null)
            {
                Image newImage;
                await using var stream = new MemoryStream(multipart.ImageArrays);

                 #pragma warning disable CA1416 // Validate platform compatibility
                using (var img = Image.FromStream(stream))
                 #pragma warning restore CA1416 // Validate platform compatibility
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
            userModel.TranslatorFirstName = data.TranslatorFirstName?.Trim();
            userModel.TranslatorLastName = data.TranslatorLastName?.Trim();
            userModel.Birthday = data.Birthday;
            userModel.Country = data.Country?.Trim();
            userModel.Language = data.Language?.Trim();
            userModel.Language = data.Language?.Trim();
            userModel.Age = Math.Abs(DateTime.Now.Year - data.Birthday.Year);

            await _translator.Update(userModel);

            if (deletedImageId.HasValue)
            {
                await _fileSystem.Delete(deletedImageId.Value);
            }
            #endregion

            return true;
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("translatorController|Updatetranslator", ex);
            return Problem(ex.Message);
        }
    }

    #endregion

    #region Delete

    [HttpGet]
    [Route("DeleteTranslator/{Id}")]
    public async Task<ActionResult<bool>> DeleteTranslator(long Id)
    {
        try
        {
            var result = await _translator.Delete(Id);
            return result;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
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

            var userModel = await _translator.GetById(userId);
            if (userModel == null || userModel.Id <= 0)
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
            var updateStatus = await _translator.Update(userModel);

            if (updateStatus >= 0)
            {
                await _fileSystem.Delete(profilePictureId);
            }

            return true;
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("TranslatorController|DeleteImage", ex);
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

            var user = await _translator.GetById(userId);
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
            LogHelper.ErrorLog("TranslatorController|GetProfilePicture", ex);
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

            var user = await _translator.GetById(userId);
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
            LogHelper.ErrorLog("TranslatorController|GetThumbnailProfilePicture", ex);
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
            LogHelper.ErrorLog("TranslatorController|GetThumbnailProfilePictureByName", ex);
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
            LogHelper.ErrorLog("TranslatorController|GetProfilePictureByName", ex);
            return NoContent();
        }
    }
    #endregion
}
