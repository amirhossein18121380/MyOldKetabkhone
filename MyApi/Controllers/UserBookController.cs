using Common.Helper;
using DataAccess.Interface.Book_related;
using DataModel.Models;
using Microsoft.AspNetCore.Mvc;
using MyApi.Controllers.Common;

namespace MyApi.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UserBookController : BaseController
{
    #region Constructor
    private readonly IUserBookDal _userbook;

    public UserBookController(IUserBookDal userbook)
    {
        _userbook = userbook;
    }
    #endregion

    #region Fetch
    [HttpGet]
    [Route("GetbyBookid")]
    //[PkPermission(ResourcesEnum.)]
    public async Task<ActionResult<UserBook>> GetbyBookid(long bookid)
    {
        try
        {
            if (UserId <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            var res = await _userbook.GetByBookId(bookid);
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
    [Route("GetByBookIdAndUserId")]
    //[PkPermission(ResourcesEnum.)]
    public async Task<ActionResult<UserBook>> GetByBookIdAndUserId(long bookid)
    {
        try
        {
            if (UserId <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            var res = await _userbook.GetByBookIdAndUserId(UserId,bookid);
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
    [Route("GetAllByUserId")]
    //[PkPermission(ResourcesEnum.)]
    public async Task<ActionResult<List<UserBook>>> GetAllByUserId()
    {
        try
        {
            if (UserId <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            var res = await _userbook.GetAllByUserId(UserId);
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
    [Route("Getlibrarybyuserid")]
    //[PkPermission(ResourcesEnum.)]
    public async Task<ActionResult<List<Book>>> Getlibrarybyuserid()
    {
        try
        {
            if (UserId <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            var res = await _userbook.GetUserLibrary(UserId);
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
    [Route("GetUserBookMarksByUserid")]
    //[PkPermission(ResourcesEnum.)]
    public async Task<ActionResult<List<Book>>> GetUserBookMarksByUserid()
    {
        try
        {
            if (UserId <= 0)
            {
                return HttpHelper.InvalidContent();
            }

            var res = await _userbook.GetUserBookMarks(UserId);
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

    #region Insert
    [HttpPost]
    [Route("AddToLibrary")]
    public async Task<ActionResult<bool>> AddToLibrary(long bookid)
    {
        try
        {
            #region Validation

            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            var userbookval = await _userbook.GetByBookIdAndUserId(UserId, bookid);

            if (userbookval?.IsAdded == true)
            {
                return HttpHelper.FailedContent("This Book Is Already Added To Library.");
            }

            #endregion
            if (userbookval?.IsMarked == true)
            {
                var getbyid = await _userbook.GetByBookIdAndUserId(UserId, bookid);

                                             #pragma warning disable CS8602 // Dereference of a possibly null reference.
                getbyid.UserId = UserId;
                                             #pragma warning restore CS8602 // Dereference of a possibly null reference.
                getbyid.BookId = bookid;
                getbyid.IsMarked = true;
                getbyid.IsAdded = true;
                getbyid.IsPurchase = false;
                getbyid.CreateOn = DateTime.Now;

                var res2Id = await _userbook.Update(getbyid);

                return true;
            }


            var res = new UserBook
            {
                UserId = UserId,
                BookId = bookid,
                IsMarked = false,
                IsAdded = true,
                IsPurchase = false,
                CreateOn = DateTime.Now,
            };

            var resId = await _userbook.Insert(res);

            if (resId <= 0)
            {
                return HttpHelper.FailedContent("Isn't possible to record a translator in this moment.");
            }

            res.Id = resId;

            return true;
        }

        catch (Exception ex)
        {
            return Problem(ex.Message);
        }

    }

    [HttpPost]
    [Route("AddToBookMarks")]
    public async Task<ActionResult<bool>> AddToBookMarks(long bookid)
    {
        try
        {
            #region Validation

            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            var userbookval = await _userbook.GetByBookIdAndUserId(UserId, bookid);

            //if (userbookval?.Result?.IsMarked == true)
            //{
            //    return HttpHelper.FailedContent("This Book Is Already Marked.");
            //}

            #endregion
            if (userbookval?.IsAdded == true)
            {
                var getbyid = await _userbook.GetByBookIdAndUserId(UserId, bookid);


#pragma warning disable CS8602 // Dereference of a possibly null reference.
                getbyid.UserId = UserId;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                getbyid.BookId = bookid;
                getbyid.IsMarked = true;
                getbyid.IsAdded = true;
                getbyid.IsPurchase = false;
                getbyid.CreateOn = DateTime.Now;


                var res2Id = await _userbook.Update(getbyid);

                return true;
            }

            var res = new UserBook
            {
                UserId = UserId,
                BookId = bookid,
                IsMarked = true,
                IsAdded = false,
                IsPurchase = false,
                CreateOn = DateTime.Now,
            };



            var resId = await _userbook.Insert(res);

            if (resId <= 0)
            {
                return HttpHelper.FailedContent("Isn't possible to record a translator in this moment.");
            }

            res.Id = resId;

            return true;
        }

        catch (Exception ex)
        {
            return Problem(ex.Message);
        }

    }

    //[HttpPost]
    //[Route("Adduserbook")]
    //public async Task<ActionResult<bool>> Adduserbook(UserBook ub)
    //{
    //    try
    //    {  
    //        #region Validation

    //        if (UserId <= 0)
    //        {
    //            return HttpHelper.AccessDeniedContent();
    //        }

    //        #endregion

    //        var res = new UserBook
    //        {
    //            UserId = ub.UserId,
    //            BookId = ub.BookId,
    //            IsMarked = ub.IsMarked,
    //            IsAdded = ub.IsAdded,
    //            IsPurchase = ub.IsPurchase,
    //            CreateOn = DateTime.Now,
    //        };

    //        var resId = await _userbook.Insert(res);

    //        if (resId <= 0)
    //        {
    //            return HttpHelper.FailedContent("Isn't possible to record a translator in this moment.");
    //        }

    //        res.Id = resId;

    //        return true;
    //    }

    //    catch (Exception ex)
    //    {
    //        return Problem(ex.Message);
    //    }

    //}

    #endregion


    #region Delete

    //[HttpGet]
    //[Route("DeleteUserBook/{Id}")]
    //public async Task<ActionResult<bool>> DeleteUserBook(long Id)
    //{
    //    try
    //    {
    //        var result = await _userbook.Delete(Id);
    //        return result;
    //    }
    //    catch (Exception ex)
    //    {
    //        return Problem(ex.Message);
    //    }
    //}

    [HttpPost]
    [Route("DeleteFromLibrary")]
    public async Task<ActionResult<bool>> DeleteFromLibrary(long bookid)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.InvalidContent();
            }


            var userbookval = await _userbook.GetByBookIdAndUserId(UserId, bookid);

            if (userbookval == null || userbookval.Id <= 0)
            {
                return HttpHelper.InvalidContent();
            }


            #endregion


            #region User Data

            userbookval.UserId = UserId;
            userbookval.BookId = bookid;
            userbookval.IsAdded = false;
            userbookval.CreateOn = DateTime.Now;

            await _userbook.Update(userbookval);

            #endregion

            return true;
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("UserbookController|DeleteFromLibrary", ex);
            return Problem(ex.Message);
        }
    }


    [HttpPost]
    [Route("DeleteFromBookMarks")]
    public async Task<ActionResult<bool>> DeleteFromBookMarks(long bookid)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.InvalidContent();
            }

            var userbookval = await _userbook.GetByBookIdAndUserId(UserId, bookid);

            if (userbookval == null || userbookval.Id <= 0)
            {
                return HttpHelper.InvalidContent();
            }


            #endregion


            #region User Data

            userbookval.UserId = UserId;
            userbookval.BookId = bookid;
            userbookval.IsMarked = false;
            userbookval.CreateOn = DateTime.Now;

            await _userbook.Update(userbookval);

            #endregion

            return true;
        }
        catch (Exception ex)
        {
            LogHelper.ErrorLog("UserbookController|DeleteFromLibrary", ex);
            return Problem(ex.Message);
        }
    }
    #endregion
}
