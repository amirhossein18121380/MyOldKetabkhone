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

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookController : BaseController
{

    #region Constructor
    private readonly IBookDal _bookDal;
    private readonly IAuthorBookDal _authorbook;
    private readonly ITranslatorBookDal _translatorbook;
    private readonly IBookSubjectDal _booksubject;
    private readonly IBookCategoryDal _bookcategory;
    private readonly IAuthorDal _author;
    private readonly ITranslatorDal _translator;
    private readonly ICategoryDal _category;
    private readonly ISubjectDal _subject;
    private readonly IFileSystem _fileSystem;

    public BookController(
        IBookDal bookDal, 
        IAuthorBookDal authorbook, 
        ITranslatorBookDal translatorbook, 
        IBookSubjectDal booksubject,
        IBookCategoryDal bookcategory,
        IAuthorDal author,
        ITranslatorDal translator,
        ICategoryDal category,
        ISubjectDal subject,
        IFileSystem fileSystem)
    {
        _bookDal = bookDal;
        _authorbook = authorbook;
        _translatorbook = translatorbook;
        _booksubject = booksubject;
        _bookcategory = bookcategory;
        _author = author;
        _translator = translator;
        _category = category;
        _subject = subject;
        _fileSystem = fileSystem;
    }

    #endregion

    #region GetById
    [HttpGet]
    [Route("GetById/{id}")]
    //[PkPermission(ResourcesEnum.BookManagement)]
    public async Task<ActionResult<BookViewModel>> GetById(long id)
    {
        try
        {
            if (id <= 0)
            {
                HttpHelper.InvalidContent();
            }

            var bookModel = await _bookDal.GetBookById(id);

            if (bookModel == null)
            {
                return HttpHelper.InvalidContent();
            }

            var authors = await _bookDal.GetAuthorsByBookId(id);
            var translators = await _bookDal.GetTranslatorByBookId(id);
            var category = await _bookDal.GetCategoryByBookId(id);
            var subject = await _bookDal.GetSubjectsByBookId(id);

            //var author = await _authorbook.getbybookid();
            //var translator = await _translatorbook.getbytranslatorid();
            //var categor = await _bookcategory.getbycategoryid();
            //var sub = await _booksubject.getbysubjectid();

            var resultModel = new BookViewModel
            {
                BookName = bookModel.BookName,
                Publisher = bookModel.Publisher,
                YearOfPublication = bookModel.YearOfPublication,
                BookFormat = bookModel.BookFormat,
                BookType = bookModel.BookType,
                NumberOfPages = bookModel.NumberOfPages,
                Language = bookModel.Language,
                ISBN = bookModel.ISBN,
                ElectronicVersionPrice = bookModel.ElectronicVersionPrice,
                BookPictureName = bookModel.BookPictureName,
                IsActive = bookModel.IsActive,
                IsDeleted = bookModel.IsDeleted,
                IsModified = bookModel.IsModified,
                LastModified = bookModel.LastModified,
                CreateOn = bookModel.CreateOn,
                Author = authors.Select(c => new Dropdownget
                {
                    AuthorId = c.Id,
                    AuthorFirstName = c.AuthorFirstName,
                    AuthorLastName = c.AuthorLastName,

                }).ToList(),
                Translator = translators.Select(c => new Dropdownget
                {
                    TranslatorId = c.Id,
                    TranslatorFirstName = c.TranslatorFirstName,
                    TranslatorLastName = c.TranslatorLastName,

                }).ToList(),
                Category = category.Select(c => new Dropdownget
                {
                    CategoryId = c.Id,
                    CategoryTitle = c.Title,

                }).ToList(),
                Subjects = subject.Select(c => new Dropdownget
                {
                    SubjectId = c.Id,
                    SubjectTitle = c.Title,

                }).ToList()
                //Author = authors.Select(c => new Author
                //{
                //    Id = c.Id,
                //}).ToList(),
                //Translator = translators.Select(c => new Translator
                //{
                //    Id = c.Id,
                //}).ToList(),
                //Category = category.Select(c => new Category
                //{
                //    Id = c.Id,
                //}).ToList(),
                //Subjects = subject.Select(c => new Subjects
                //{
                //    Id = c.Id,
                //}).ToList()
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

    #region Fetch
    [HttpPost]
    [Route("GetList")]
    //[IgnoreAntiforgeryToken]
    [PkPermission(ResourcesEnum.BookManagement)]
    public async Task<ActionResult<PagedResponse<List<BookListViewModel>>>> GetList(BookGetListFilterViewModel filterModel)
    {
        try
        
        {
            var users = await _bookDal.GetList(filterModel);

            if (!users.data.Any())
            {
                return HttpHelper.InvalidContent();
            }

            var result = new PagedResponse<List<BookListViewModel>>(users.data, users.totalCount);
            return result;
        }
        catch (Exception ex)
        {
            //await MongoLogging.ErrorLogAsync("UserController|GetList", ex.Message, ex.StackTrace);
            return HttpHelper.ExceptionContent(ex);
        }
    }


   
    [HttpGet]
    [Route("GetByAuthor")]
    public async Task<ActionResult<Author>> GetByAuthor(string author)
    {
        try
        {
            var res = await _bookDal.GetByAuthor(author);
            if (res == null)
            {
                return HttpHelper.InvalidContent();
            }
            return Ok(res);
        }
        catch (Exception ex)
        {
            return HttpHelper.ExceptionContent(ex);
        }
    }

    [HttpGet]
    [Route("GetByBookName")]
    public async Task<ActionResult<Author>> GetByBookName(string bookname)
    {
        try
        {
            var res = await _bookDal.GetByBookName(bookname);
            if (res == null)
            {
                return HttpHelper.InvalidContent();
            }
            return Ok(res);
        }
        catch (Exception ex)
        {
            return HttpHelper.ExceptionContent(ex);
        }
    }

    [HttpGet]
    [Route("GetByPublisher")]
    public async Task<ActionResult<Author>> GetByPublisher(string publisher)
    {
        try
        {
            var res = await _bookDal.GetByPublisher(publisher);
            if (res == null)
            {
                return HttpHelper.InvalidContent();
            }
            return Ok(res);
        }
        catch (Exception ex)
        {
            return HttpHelper.ExceptionContent(ex);
        }
    }

    [HttpGet]
    [Route("GetByBookSubject")]
    public async Task<ActionResult<Author>> GetByBookSubject(string subject)
    {
        try
        {
            var res = await _bookDal.GetByBookSubject(subject);
            if (res == null)
            {
                return HttpHelper.InvalidContent();
            }
            return Ok(res);
        }
        catch (Exception ex)
        {
            return HttpHelper.ExceptionContent(ex);
        }
    }

    [HttpGet]
    [Route("GetByTranslator")]
    public async Task<ActionResult<Author>> GetByTranslator(string translator)
    {
        try
        {
            var res = await _bookDal.GetByTranslator(translator);
            if (res == null)
            {
                return HttpHelper.InvalidContent();
            }
            return Ok(res);
        }
        catch (Exception ex)
        {
            return HttpHelper.ExceptionContent(ex);
        }
    }

    #endregion

    #region AddBook
    [HttpPost]
    [Route("AddBook")]
    public async Task<ActionResult<bool>> AddBook()
    {
        #region Validation
        //var validation = BookValidation.ApplyBookValidator(bookvm);
        //if (validation.ValidationMessages.Count > 0 & validation.ValidationMessages != null)
        //{
        //    return BadRequest(validation.ValidationMessages);
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
        var data = json.ToObject<AddBookViewModel>();

        if (data == null)
        {
            return HttpHelper.InvalidContent();
        }
        #endregion

        //if (UserId <= 0)
        //{
        //    return HttpHelper.AccessDeniedContent();
        //}

        //var isbn = await _bookDal.GetByISBN(bookvm.ISBN);
        //if (isbn != null)
        //{
        //    return BadRequest("This ISBN is Already in used.");
        //}

        #region Insert User Data
        var bookmodel = new Book
        {
            ParentId = null,
            BookName = data.BookName,
            //Author = bookvm.Author,
            //Translator = bookvm.Translator,
            Publisher = data.Publisher,
            YearOfPublication = data.YearOfPublication,
            BookFormat = (short)BookFormatStatus.Epub,
            BookType = (short)BookTypeStatus.ElectronicBook,
            NumberOfPages = data.NumberOfPages,
            Language = data.Language,
            ISBN = data.ISBN,
            ElectronicVersionPrice = data.ElectronicVersionPrice,
            IsActive = data.IsActive,
            //IsDeleted = bookvm.IsDeleted,
            CreateOn = DateTime.UtcNow,
        };

        var bookId = await _bookDal.AddBook(bookmodel);

        if (bookId <= 0)
        {
            return HttpHelper.FailedContent("Isn't possible to record a book in this moment.");
        };

        bookmodel.Id = bookId;
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

            var fileName = $"PI_{bookmodel.Id}_{DateTime.Now.Ticks}";
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
                bookmodel.BookPictureId = pictureImageId;
                bookmodel.BookPictureName = fileName;

                await _bookDal.UpdateBook(bookmodel);
            }
        }
        #endregion



        #region set author-book

        foreach (var bids in data.authorids)
        {
            await _authorbook.Insert(new AuthorBook
            {
                AuthorId = bids,
                BookId = bookId,
            });
        }

        #endregion

        #region set translator-book

        foreach (var bids in data.translatorids)
        {
            await _translatorbook.Insert(new TranslatorBook
            {
                TranslatorId = bids,
                BookId = bookId
            });
        }

        #endregion

        #region set book-subject

        foreach (var bids in data.booksubjectids)
        {
            await _booksubject.Insert(new BookSubject
            {
                SubjectId = bids,
                BookId = bookId
            });
        }


        #endregion

        #region set book-category

        foreach (var bids in data.bookcategoryids)
        {
            await _bookcategory.Insert(new BookCategory
            {
                CategoryId = bids,
                BookId = bookId
            });
        }

        #endregion


        return bookId > 0;
    }

    #endregion

    #region UploadImage

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

            var user = await _bookDal.GetBookById(userId);
            if (user == null)
                return NotFound();

            if (user.BookPictureId.HasValue)
            {
                var fileModel = await _fileSystem.GetById(user.BookPictureId.Value);

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
    [Route("GetThumbnailProfilePicture/{bookId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetThumbnailProfilePicture(long bookId)
    {
        try
        {
            if (bookId <= 0)
            {
                return NotFound();
            }

            var user = await _bookDal.GetBookById(bookId);
            if (user == null)
                return NotFound();

            if (user.BookPictureId.HasValue)
            {
                var thumbnailFileModel = await _fileSystem.GetById(user.BookPictureId.Value);

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
            var userModel = await _bookDal.GetBookById(UserId);

            if (userModel == null || userModel.Id <= 0)
            {
                return HttpHelper.InvalidContent();
            }

            #endregion

            #region Save Picture                

            var deletedImageId = (long?)null;
            if (userModel.BookPictureId.HasValue)
            {
                deletedImageId = userModel.BookPictureId.Value;
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

            userModel.BookPictureId = pictureImageId;
            userModel.BookPictureName = fileName;
            var updateState = await _bookDal.UpdateBook(userModel);

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
            LogHelper.ErrorLog("bookController|uploadimage|SetProfileImage", ex);
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
            var userModel = await _bookDal.GetBookById(UserId);

            if (userModel == null || userModel.Id <= 0)
            {
                return HttpHelper.InvalidContent();
            }

            #endregion

            #region Save Picture                

            var deletedImageId = (long?)null;
            if (userModel.BookPictureId.HasValue)
            {
                deletedImageId = userModel.BookPictureId.Value;
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

            userModel.BookPictureId = pictureImageId;
            userModel.BookPictureName = fileName;
            var updateState = await _bookDal.UpdateBook(userModel);

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
            LogHelper.ErrorLog("bookController|UploadImage", ex);
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

            var userModel = await _bookDal.GetBookById(userId);
            if (userModel == null || userModel.Id <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            if (!userModel.BookPictureId.HasValue)
            {
                return true;
            }
            var profilePictureId = userModel.BookPictureId.Value;
            userModel.BookPictureId = null;
            userModel.BookPictureName = null;
            var updateStatus = await _bookDal.UpdateBook(userModel);

            if (updateStatus >= 0)
            {
                await _fileSystem.Delete(profilePictureId);
            }

            return true;
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("UserController|DeleteImage", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    //[HttpPost]
    //    [Route("AddBookToBookMarks")]
    //    public async Task<ActionResult<bool>> AddBookToBookMarks(BookVm bookvm)
    //    {
    //        #region Validation
    //        var validation = BookValidation.ApplyBookValidator(bookvm);
    //        if (validation.ValidationMessages.Count > 0 & validation.ValidationMessages != null)
    //        {
    //            return BadRequest(validation.ValidationMessages);
    //        }
    //        #endregion
    //        var isbn = await _bookDal.GetByISBN(bookvm.ISBN);
    //        if (isbn != null)
    //        {
    //            return BadRequest("This ISBN is Already in used.");
    //        }

    //        var book = new Book
    //        {
    //            ParentId = null,
    //            BookName = bookvm.BookName,
    //            Author = bookvm.Author,
    //            Translator = bookvm.Translator,
    //            Publisher = bookvm.Publisher,
    //            YearOfPublication = bookvm.YearOfPublication,
    //            BookFormat = (short)BookFormatStatus.Epub,
    //            BookType = (short)BookTypeStatus.ElectronicBook,
    //            NumberOfPages = bookvm.NumberOfPages,
    //            Language = bookvm.Language,
    //            ISBN = bookvm.ISBN,
    //            BookSubject = bookvm.BookSubject,
    //            ElectronicVersionPrice = bookvm.ElectronicVersionPrice,
    //            BookPictureName = bookvm.BookName,
    //            BookPictureId = bookvm.BookPictureId,
    //            IsActive = true,
    //            IsDeleted = false,
    //            IsModified = false,
    //            ModifierId = null,
    //            LastModified = null,
    //            CreateOn = DateTime.UtcNow,
    //        };

    //        var userId = await _bookDal.AddBook(book);

    //        if (userId <= 0)
    //        {
    //            return HttpHelper.FailedContent("Isn't possible to record a book in this moment.");
    //        }

    //        //author.Id = userId;

    //        return userId > 0;

    //    }

    #region update

    [HttpPost]
    [Route("UpdateBook")]
    public async Task<ActionResult<bool>> UpdateBook()
    {
        try
        {
            //var validation = BookValidation.ApplyBookValidator(bookvm);
            //if (validation.ValidationMessages.Count > 0 & validation.ValidationMessages != null)
            //{
            //    return BadRequest(validation.ValidationMessages);
            //}


            //var isbn = await _bookDal.GetByISBN(bookvm.ISBN);
            //if (isbn != null)
            //{
            //    return BadRequest("This ISBN is Already in used.");
            //}

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
            var data = json.ToObject<UpdateBookViewModel>();

            if (data == null)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            if (data.Id <= 0)
            {
                return HttpHelper.InvalidContent();
            }

            var bookModel = await _bookDal.GetBookById(data.Id);

            if (bookModel == null || bookModel.Id <= 0)
            {
                return HttpHelper.InvalidContent();
            }

            bookModel.BookName = data.BookName;
            bookModel.Publisher = data.Publisher;
            bookModel.YearOfPublication = data.YearOfPublication;
            bookModel.BookFormat = data.BookFormat;
            bookModel.BookType = data.BookType;
            bookModel.NumberOfPages = data.NumberOfPages;
            bookModel.Language = data.Language;
            bookModel.ISBN = data.ISBN;
            bookModel.ElectronicVersionPrice = data.ElectronicVersionPrice;
            bookModel.IsActive = data.IsActive;
            bookModel.CreateOn = DateTime.UtcNow;

            var bookres = await _bookDal.UpdateBook(bookModel);

            if (bookres <= 0)
            {
                return HttpHelper.FailedContent("Isn't possible to record a book in this moment.");
            };


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

                var fileName = $"PI_{bookModel.Id}_{DateTime.Now.Ticks}";
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
                    bookModel.BookPictureId = pictureImageId;
                    bookModel.BookPictureName = fileName;

                    await _bookDal.UpdateBook(bookModel);
                }
            }
            #endregion

            #region set author-book
            await _authorbook.DeleteByBookId(bookModel.Id);

            foreach (var bids in data.authorids)
            {
                await _authorbook.Insert(new AuthorBook
                {
                    BookId = bookModel.Id,
                    AuthorId = bids,
                });
            }

            #endregion

            #region set translator-book

            foreach (var bids in data.translatorids)
            {
                await _translatorbook.DeleteByBookId(bookModel.Id);
                await _translatorbook.Insert(new TranslatorBook
                {
                    TranslatorId = bids,
                    BookId = bookModel.Id,
                });
            }

            #endregion

            #region set book-subject

            foreach (var bids in data.booksubjectids)
            {
                await _booksubject.DeleteByBookId(bookModel.Id);

                await _booksubject.Insert(new BookSubject
                {
                    SubjectId = bids,
                    BookId = bookModel.Id,
                });
            }


            #endregion

            #region set book-category

            foreach (var bids in data.bookcategoryids)
            {
                await _bookcategory.DeleteByBookId(bookModel.Id);

                await _bookcategory.Insert(new BookCategory
                {
                    CategoryId = bids,
                    BookId = bookModel.Id,
                });
            }

            #endregion


            return true;

        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message);
        }
    }

    #endregion    
}