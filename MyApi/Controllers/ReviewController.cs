using Common.Enum;
using Common.Helper;
using DataAccess.Interface;
using DataModel.Models;
using DataModel.ViewModel;
using DataModel.ViewModel.Common;
using Microsoft.AspNetCore.Mvc;
using MyApi.Controllers.Common;

namespace MyApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : BaseController
{
    #region Constructor
    private readonly IReviewDal _review;

    public ReviewController(IReviewDal review)
    {
        _review = review;
    }
    #endregion

    #region Fetch
    [HttpPost]
    [Route("GetList")]
    //[IgnoreAntiforgeryToken]
    //[PkPermission(ResourcesEnum.AuthorManagement)]
    public async Task<ActionResult<PagedResponse<List<Review>>>> GetList(ReviewGetListViewModel filterModel)
    {
        try
        {
            var comments = await _review.GetList(filterModel);

            if (!comments.data.Any())
            {
                return HttpHelper.InvalidContent();
            }

            var result = new PagedResponse<List<Review>>(comments.data, comments.totalCount);
            return result;
        }
        catch (Exception ex)
        {
            //await MongoLogging.ErrorLogAsync("UserController|GetList", ex.Message, ex.StackTrace);
            return HttpHelper.ExceptionContent(ex);
        }
    }

    [HttpPost]
    [Route("GetParentList")]
    //[IgnoreAntiforgeryToken]
    //[PkPermission(ResourcesEnum.TranslatorManagement)]
    public async Task<ActionResult<PagedResponse<List<Review>>>> GetParentList(ReviewGetListViewModel filterModel)
    {
        try
        {
            var users = await _review.GetParentList(filterModel);

            if (!users.data.Any())
            {
                return HttpHelper.InvalidContent();
            }

            var result = new PagedResponse<List<Review>>(users.data, users.totalCount);
            return result;
        }
        catch (Exception ex)
        {
            //await MongoLogging.ErrorLogAsync("UserController|GetList", ex.Message, ex.StackTrace);
            LogHelper.ErrorLog("ReviewController|GetParentList", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }

    [HttpPost]
    [Route("GetChildrenList")]
    //[IgnoreAntiforgeryToken]
    //[PkPermission(ResourcesEnum.TranslatorManagement)]
    public async Task<ActionResult<PagedResponse<List<Review>>>> GetChildrenList(ReviewGetListViewModel filterModel)
    {
        try
        {
            var users = await _review.GetChildrenList(filterModel);

            if (!users.data.Any())
            {
                return HttpHelper.InvalidContent();
            }

            var result = new PagedResponse<List<Review>>(users.data, users.totalCount);
            return result;
        }
        catch (Exception ex)
        {
            //await MongoLogging.ErrorLogAsync("UserController|GetList", ex.Message, ex.StackTrace);
            LogHelper.ErrorLog("ReviewController|GetChildrenList", ex);
            return HttpHelper.ExceptionContent(ex);
        }
    }


    [HttpGet]
    [Route("GetParents/{entitytype}")]
    public async Task<ActionResult<List<Review>>> GetParents(short entitytype)
    {
        try
        {

            var results = await _review.GetParents(entitytype);
            return results == null ? (ActionResult<List<Review>>)NotFound() : Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }


    [HttpGet]
    [Route("GetChilds/{entitytype}/{entityId}")]
    public async Task<ActionResult<List<Review>>> GetChilds(short entitytype, long entityId)
    {
        try
        {
            var results = await _review.GetChildsByTypeAndId(entitytype, entityId);
            return results == null ? (ActionResult<List<Review>>)NotFound() : Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost]
    [Route("GetComment")]
    public async Task<ActionResult<Review>> GetComment(CommentSendViewModel coview)
    {
        try
        {
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            var com = new CommentViewModel
            {
                UserId = UserId,
                EntityType = coview.EntityType,
                EntityId = coview.EntityId,
            };

            var results = await _review.GetComment(com);

            return results == null ? (ActionResult<Review>)NotFound() : Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);

        }
    }


    [HttpGet]
    [Route("GetById/{id}")]
    public async Task<ActionResult<Review>> GetById(long id)
    {
        try
        {
            var results = await _review.GetById(id);
            if (results == null) return NotFound();
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
    [Route("AddReview")]
    public async Task<ActionResult<bool>> AddReview(SendCommentViewModel reviewvm)
    {
        try
        {
            if (UserId <= 0)
            {
                HttpHelper.InvalidContent();
            }

            var asn = new Review()
            {
                ParentId = reviewvm.ParentId,
                UserId = UserId,
                EntityType = reviewvm.EntityType,
                EntityId = reviewvm.EntityId,
                CommentValue = reviewvm.CommentValue,
                CommentDate = DateTime.Now,
            };
            var result = await _review.Insert(asn);

            if (result <= 0)
            {
                return HttpHelper.FailedContent("Isn't possible to like in this moment.");
            }

            asn.Id = result;


            return true;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }


    [HttpPost]
    [Route("AddPanelReview")]
    public async Task<ActionResult<bool>> AddPanelReview(SendPanelReviewViewModel reviewvm)
    {
        try
        {
            //if (UserId <= 0)
            //{
            //    HttpHelper.InvalidContent();
            //}

            var asn = new Review()
            {
                ParentId = reviewvm.ParentId,
                UserId = reviewvm.UserId,
                EntityType = reviewvm.EntityType,
                EntityId = reviewvm.EntityId,
                CommentValue = reviewvm.CommentValue,
                CommentDate = DateTime.Now,
            };
            var result = await _review.Insert(asn);

            if (result <= 0)
            {
                return HttpHelper.FailedContent("Isn't possible to like in this moment.");
            }

            asn.Id = result;


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
    [Route("UpdateReview")]
    public async Task<ActionResult<bool>> UpdateReview(SendCommentViewModel comsend)
    {
        try
        {
            if (UserId <= 0)
            {
                HttpHelper.InvalidContent();
            }

            var comment = new CommentViewModel
            {
                UserId = UserId,
                EntityType = comsend.EntityType,
                EntityId = comsend.EntityId,
            };

            var commentModel = await _review.GetComment(comment);

                                     #pragma warning disable CS8602 // Dereference of a possibly null reference.
            commentModel.CommentValue = comsend.CommentValue;
                                     #pragma warning restore CS8602 // Dereference of a possibly null reference.
            commentModel.CommentDate = DateTime.Now;


            var comId = await _review.Update(commentModel);

            if (comId <= 0)
            {
                return HttpHelper.FailedContent("Isn't possible to record a user in this moment.");
            }

            return comId > 0;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
            //return StatusCode(500, ex.Message);
        }
    }


    [HttpPost]
    [Route("UpdatePanelReview")]
    public async Task<ActionResult<bool>> UpdatePanelReview(UpdatePanelViewModel comsend)
    {
        try
        {
            //if (UserId <= 0)
            //{
            //    HttpHelper.InvalidContent();
            //}

            var comment = new CommentViewModel
            {
                UserId = comsend.UserId,
                EntityType = comsend.EntityType,
                EntityId = comsend.EntityId,
            };

            var commentModel = await _review.GetComment(comment);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            commentModel.CommentValue = comsend.CommentValue;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            commentModel.CommentDate = DateTime.Now;


            var comId = await _review.Update(commentModel);

            if (comId <= 0)
            {
                return HttpHelper.FailedContent("Isn't possible to record a user in this moment.");
            }

            return comId > 0;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
            //return StatusCode(500, ex.Message);
        }
    }
    #endregion

    #region Delete


    [HttpPost]
    [Route("Delete")]
    public async Task<ActionResult<bool>> Delete(CommentSendViewModel coview)
    {
        try
        {
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            var like = new CommentViewModel 
            {
                UserId = UserId,
                EntityType = coview.EntityType,
                EntityId = coview.EntityId,
            };

            var result = await _review.DeleteBy(like);
            return result;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    #endregion
}
