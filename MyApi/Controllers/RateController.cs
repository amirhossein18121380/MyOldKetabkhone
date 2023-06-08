using Common.Enum;
using Common.Helper;
using DataAccess.Interface;
using DataModel.Models;
using DataModel.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MyApi.Controllers.Common;

namespace MyApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RateController : BaseController
{
    #region Constructor
    private readonly IRateDal _rate;

    public RateController(IRateDal rate)
    {
        _rate = rate;
    }
    #endregion

    #region Fetch
    [HttpGet]
    [Route("GetAllRates")]
    public async Task<ActionResult<List<Rate>>> GetAllRates(int entitytype)
    {
        try
        {
            var companies = await _rate.GetAll(entitytype);
            return Ok(companies);
        }
        catch (Exception ex)
        {
            //log error
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("GetTheAvrRateByEntityIdAndEntityType/{entitytype}/{entityid}")]
    public async Task<ActionResult<Rate>> GetTheAvrRateByEntityIdAndEntityType(int entitytype, long entityid)
    {
        try
        {
            var results = await _rate.GetTheAvrRateByEntityIdAndEntityType(entitytype, entityid);
            if (results == null) return NotFound();
            return Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetTheAvrRateByEntityIdForAuthors/{authorid}")]
    public async Task<ActionResult<Rate>> GetTheAvrRateByEntityIdForAuthors(long authorid)
    {
        try
        {
            var results = await _rate.GetTheAvrRateByEntityIdForAuthors(authorid);
            return results == null ? NotFound() : (ActionResult<Rate>)results;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetTheAvrRateByEntityIdForTranslators/{translatorid}")]
    public async Task<ActionResult<Rate>> GetTheAvrRateByEntityIdForTranslators(long translatorid)
    {
        try
        {
            var results = await _rate.GetTheAvrRateByEntityIdForTranslators(translatorid);
            return results == null ? NotFound() : (ActionResult<Rate>)results;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetTheAvrRateByEntityIdForUser/{userid}")]
    public async Task<ActionResult<Rate>> GetTheAvrRateByEntityIdForUser(long userid)
    {
        try
        {
            var results = await _rate.GetTheAvrRateByEntityIdForUser(userid);
            return results == null ? NotFound() : results;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetTheAvrRateByEntityIdForBook/{bookid}")]
    public async Task<ActionResult<decimal>> GetTheAvrRateByEntityIdForBook(long bookid)
    {
        try
        {
            var results = await _rate.GetTheAvrRateByEntityIdForBook(bookid);
            
            return results == null ? NotFound() : Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetRateByEntityIdandEntityType/{entitytype}/{entityid}")]
    public async Task<ActionResult<Rate>> GetRateByEntityIdandEntityType(short entitytype, long entityid)
    {
        try
        {
            var results = await _rate.GetRateByEntityTypeandEntityId(entitytype, entityid);
            return results == null ? (ActionResult<Rate>)NotFound() : Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost]
    [Route("GetRate")]
    public async Task<ActionResult<Rate>> GetRate(GetRateSendViewModel getrate)
    {
        try
        {
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            var rate = new GetRateViewModel
            {
                UserId = UserId,
                EntityType = getrate.EntityType,
                EntityId = getrate.EntityId,
            };
            var results = await _rate.GetRate(rate);

            return results == null ? (ActionResult<Rate>)NotFound() : Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetRateById/{id}")]
    public async Task<ActionResult<Rate>> GetRateById(long id)
    {
        try
        {
            var results = await _rate.GetById(id);
            return results == null ? (ActionResult<Rate>)NotFound() : Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    #endregion

    #region Add

    [HttpPost]
    [Route("AddRate")]
    public async Task<ActionResult<bool>> ChangeRate(RateViewModel rate)
    {
        try
        {
            #region Validation

            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }
            #endregion

            var res = new GetRateViewModel
            {
                UserId = UserId,
                EntityType = rate.EntityType,
                EntityId = rate.EntityId,
            };

            var rateModel = await _rate.GetRate(res);

            if (rateModel == null || rateModel.Id <= 0)
            {
                var rr = new Rate
                {
                    UserId = UserId,
                    //EntityType = (short)(EntityTypeStatus.Author),
                    EntityType = rate.EntityType,
                    EntityId = rate.EntityId,
                    RateValue = rate.RateValue,
                    CreateOn = DateTime.Now,
                };

                var result = await _rate.Insert(rr);

                if (result <= 0)
                {
                    return HttpHelper.FailedContent("Isn't possible to apply  rate in this moment.");
                }

                rr.Id = result;


                return true;
            }
            else
            {
                rateModel.RateValue = rate.RateValue;
                rateModel.CreateOn = DateTime.Now;

                var result = await _rate.Update(rateModel);

                if (result <= 0)
                {
                    return HttpHelper.FailedContent("Something wrong in changing the rate.");
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

    //[HttpGet]
    //[Route("Deleterate/{Id}")]
    //public async Task<ActionResult<bool>> Deleterate(long Id)
    //{
    //    try
    //    {
    //        var result = await _rate.Delete(Id);
    //        return result;
    //    }
    //    catch (Exception ex)
    //    {
    //        return Problem(ex.Message);
    //    }
    //}
    #endregion
}
