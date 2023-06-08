using Common.Helper;
using DataAccess.Interface;
using DataModel.Models;
using DataModel.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MyApi.Controllers.Common;

namespace MyApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LikeController : BaseController
{
    #region Constructor
    private readonly ILikeDal _like;

    public LikeController(ILikeDal like)
    {
        _like = like;
    }
    #endregion

    #region Fetch
    [HttpPost]
    [Route("GetLike")]
    public async Task<ActionResult<Like>> GetLike(GetLikeSendViewModel likeview)
    {
        try
        {
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            var like = new LikeViewModel
            {
                UserId = UserId,
                EntityType = likeview.EntityType,
                EntityId = likeview.EntityId,
            };

            var results = await _like.GetLike(like);

            return results == null ? (ActionResult<Like>)NotFound() : Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }


    [HttpGet]
    [Route("GetTotalLikeByEntityIdAndEntityType/{entitytype}/{entityid}")]
    public async Task<ActionResult<Like>> GetTotalLikeByEntityIdAndEntityType(short entitytype, long entityid)
    {
        try
        {
            var results = await _like.GetTotalLikeByEntityIdAndEntityType(entitytype, entityid);
            if (results == null) return NotFound();
            return Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    #endregion

    #region Add

    [HttpPost]
    [Route("AddLike")]
    public async Task<ActionResult<bool>> AddLike(ChangeLikeViewModel dlview)
    {
        try
        {
            #region Validation

            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }
            #endregion
            var like = new LikeViewModel
            {
                UserId = UserId,
                EntityType = dlview.EntityType,
                EntityId = dlview.EntityId,
            };

            var likeModel = await _like.GetLike(like);

            if (likeModel == null || likeModel.Id <= 0)
            {
                var rr = new Like
                {
                    UserId = UserId,
                    EntityType = dlview.EntityType,
                    EntityId = dlview.EntityId,
                    Type = dlview.Type,
                    CreateOn = DateTime.Now,
                };

                var result = await _like.Insert(rr);

                if (result <= 0)
                {
                    return HttpHelper.FailedContent("Isn't possible to like in this moment.");
                }

                rr.Id = result;


                return true;
            }
            else
            {
                likeModel.Type = dlview.Type;
                likeModel.CreateOn = DateTime.Now;

                var update = await _like.Update(likeModel);
                //var result = await _like.DeleteBy(like);

                //if (result == false)
                //{
                //    return HttpHelper.FailedContent("Something wrong in deleting the like.");
                //}

                return true;
            }
        }

        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost]
    [Route("ChangeLike")]
    public async Task<ActionResult<bool>> ChangeLike(ChangeLikeViewModel dlview)
    {
        try
        {
            #region Validation

            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }
            #endregion
            var like = new LikeViewModel
            {
                UserId = UserId,
                EntityType = dlview.EntityType,
                EntityId = dlview.EntityId,
            };

            var likeModel = await _like.GetLike(like);

            if (likeModel == null || likeModel.Id <= 0)
            {
                var rr = new Like
                {
                    UserId = UserId,
                    //EntityType = (short)(EntityTypeStatus.Author),
                    EntityType = dlview.EntityType,
                    EntityId = dlview.EntityId,
                    Type = dlview.Type,
                    CreateOn = DateTime.Now,
                };

                var result = await _like.Insert(rr);

                if (result <= 0)
                {
                    return HttpHelper.FailedContent("Isn't possible to like in this moment.");
                }

                rr.Id = result;


                return true;
            }
            else
            {
                likeModel.Type = dlview.Type;
                likeModel.CreateOn = DateTime.Now;

                var result = await _like.Update(likeModel);

                if (result <= 0)
                {
                    return HttpHelper.FailedContent("Something wrong in changing the like.");
                }

                return true;
            }

        }

        catch (Exception ex)
        {
            return Problem(ex.Message);
        }

    }

    #endregion

    //#region Update
    //[HttpPost]
    //[Route("ChangeRate")]
    //public async Task<ActionResult<bool>> UpdateRate(RateViewModel rate)
    //{
    //    try
    //    {
    //        #region Validation
    //        if (UserId <= 0)
    //        {
    //            return HttpHelper.AccessDeniedContent();
    //        }


    //        var getrate = new GetRateViewModel
    //        {
    //            UserId = UserId,
    //            EntityType = rate.EntityType,
    //            EntityId = rate.EntityId,
    //        };

    //        var rateModel = await _rate.GetRate(getrate);

    //        if (rateModel == null || rateModel.Id <= 0)
    //        {
    //            return HttpHelper.InvalidContent();
    //        }

    //        #endregion

    //        #region User Profile Data
    //        rateModel.RateValue = rate.RateValue;
    //        rateModel.CreateOn = DateTime.Now;

    //        var result = await _rate.Update(rateModel);

    //        if (result <= 0)
    //        {
    //            return HttpHelper.FailedContent("Something wrong in changing the rate.");
    //        }

    //        #endregion

    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        LogHelper.ErrorLog("RateController|ChangeRate", ex);
    //        return Problem(ex.Message);
    //    }
    //}

    //#endregion

    #region Delete

    [HttpGet]
    [Route("Deleterate/{Id}")]
    public async Task<ActionResult<bool>> Deleterate(long Id)
    {
        try
        {

            var result = await _like.Delete(Id);
            return result;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }


    [HttpPost]
    [Route("DeleteBy")]
    public async Task<ActionResult<bool>> DeleteBy(GetLikeSendViewModel likeview)
    {
        try
        {
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            var like = new LikeViewModel
            {
                UserId = UserId,
                EntityType = likeview.EntityType,
                EntityId = likeview.EntityId,
            };

            var result = await _like.DeleteBy(like);
            return result;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    #endregion
}