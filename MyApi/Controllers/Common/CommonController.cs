using System.Drawing;
using Common.Enum;
using Common.Extension;
using Common.Helper;
using DataAccess.DAL.Common;
using DataAccess.Tool;
using DataModel.Common;
using DataModel.ViewModel.Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Helpers;

namespace MyApi.Controllers.Common;

[Route("api/[controller]")]
[ApiController]
public class CommonController : BaseController
{
    #region Constructor
    private readonly IFileSystem _fileSystem;

    public CommonController(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
    #endregion

    #region File
    [HttpPost]
    [Route("AddImage")]
    [RegisterUpload]
    [IgnoreAntiforgeryToken]
    [AllowAnonymous]
    public async Task<ActionResult<KeyValueViewModel>> AddImage([FromForm(Name = "image")] IFormFile img)
    {
        try
        {
            var imageArrays = await img.ToByteArray();
            var fileExtension = img.FileName.GetFileExtension();
            var fileName = $"IMG_{StringHelper.GenarateTrCode()}_{DateTime.Now.Ticks}";

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
                return HttpHelper.FailedContent("خطا در ذخیره سازی تصویر");
            }

            return new KeyValueViewModel(pictureImageId, fileName);
        }
        catch (Exception ex)
        {
            await Logging2.ErrorLogAsync("CommonController|AddImage", ex.Message, ex.StackTrace);
            return HttpHelper.ExceptionContent(ex);
        }
    }

    [Route("GetUserImage/{fileName}")]
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetUserImage(string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName.Trim()))
            {
                return HttpHelper.InvalidContent();
            }

            var imgPath = fileName.Trim().GetFilePath();

            if (string.IsNullOrEmpty(imgPath.Key))
            {
                return HttpHelper.InvalidContent();
            }


            var mediaType = $"image/{imgPath.Value.Substring(1, imgPath.Value.Length - 1)}";

            return PhysicalFile(imgPath.Key, mediaType);
        }
        catch (Exception exp)
        {
            //MongoLogging.ErrorLogSync("CommonController|GetUserImage", exp.Message, exp.StackTrace);
            return HttpHelper.ExceptionContent(exp);
        }
    }

    [HttpGet]
    [Route("GetImageById/{imageId}")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult?> GetImageById(long imageId)
    {
        try
        {
            if (imageId <= 0)
            {
                return NotFound();
            }

            var fileModel = await _fileSystem.GetById(imageId);

            if (fileModel == null)
            {
                return NoContent();
            }

            if (fileModel.FileData != null)
            {
                return File(fileModel.FileData, "application/octet-stream", $"{fileModel.FileName}{fileModel.FileType}");
            }

            return null;
        }
        catch (Exception exp)
        {
            //await MongoLogging.ErrorLogAsync("CommonController|GetImageById", exp.Message, exp.StackTrace);
            return NoContent();
        }
    }

    [HttpGet]
    [Route("GetThumbnailImageById/{imageId}")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult?> GetThumbnailImageById(long imageId)
    {
        try
        {
            if (imageId <= 0)
            {
                return NotFound();
            }

            var fileModel = await _fileSystem.GetById(imageId);

            if (fileModel == null)
            {
                return NoContent();
            }

            if (fileModel.ThumbnailFileData != null)
            {
                return File(fileModel.ThumbnailFileData, "application/octet-stream", $"{fileModel.ThumbnailFileName}{fileModel.FileType}");
            }

            return null;
        }
        catch (Exception exp)
        {
            //await MongoLogging.ErrorLogAsync("CommonController|GetThumbnailImageById", exp.Message, exp.StackTrace);
            return NoContent();
        }
    }
    #endregion

    #region Log
    [HttpPost]
    [Route("GetLogList")]
    [IgnoreAntiforgeryToken]
    [PkPermission(ResourcesEnum.LogViewer)]
    public async Task<ActionResult<PagedResponse<List<LogResponseViewModel>>>> GetLogList(LogFilterViewModel filterModel)
    {
        try
        {
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            var logs = await LogDal.GetLogs(filterModel);

            var items = logs.items.Select(c => new LogResponseViewModel
            {
                Message = c.Message,
                Level = c.Level,
                StackTrace = c.StackTrace,
                CreateDate = c.CreateDate.ToLocalTime(),
                MethodName = c.MethodName,
                LevelTitle = ((LogLevelEnum)c.Level).ToString()
            }).ToList();

            var result = new PagedResponse<List<LogResponseViewModel>>(items, (int)logs.totalCount);
            return result;
        }
        catch (Exception exp)
        {
            await Logging2.ErrorLogAsync("CommonController|GetLogList", exp.Message, exp.StackTrace);
            return HttpHelper.ExceptionContent(exp);
        }
    }
    #endregion
}
