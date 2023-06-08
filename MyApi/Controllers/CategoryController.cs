#region usings
using DataAccess.Interface;
using DataModel.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
#endregion

namespace MyApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    #region Constructor
    private readonly ICategoryDal _category;

    public CategoryController(ICategoryDal category)
    {
        _category = category;
    }
    #endregion

    #region Fetch
    [HttpGet]
    [Route("GetAllCategories")]
    public async Task<ActionResult<List<Category>>> GetAllCategories()
    {
        try
        {
            var companies = await _category.GetAll();
            return Ok(companies);
        }
        catch (Exception ex)
        {
            //log error
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("GetCategoryById/{id}")]
    public async Task<ActionResult<Category>> GetCategoryById(long id)
    {
        try
        {
            var results = await _category.GetById(id);
            if (results == null) return NotFound();
            return Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetChild/{parentid}")]
    public async Task<ActionResult<Category>> GetChild(long parentid)
    {
        try
        {
            var results = await _category.GetChild(parentid);
            if (results == null) return NotFound();
            return Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetParent")]
    public async Task<ActionResult<Category>> GetParent()
    {
        try
        {
            var results = await _category.GetParent();
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
    [Route("AddCategory")]
    public async Task<ActionResult<bool>> AddCategory(Category category)
    {
        try
        {
            var result = await _category.Insert(category);

            return result;
            //return result.Id;
            // return Ok(result);
            //return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost]
    [Route("AddChild")]
    public async Task<ActionResult<bool>> AddChild(Category category)
    {
        try
        {
            var result = await _category.AddChild(category);

            return result;
            //return result.Id;
            // return Ok(result);
            //return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    #endregion

    #region Update

    [HttpPost]
    [Route("UpdateCategory")]
    public async Task<ActionResult<bool>> UpdateCategory(Category category)
    {
        try
        {
            //var tes = await data.GetStudentById(student.Id);
            // if(tes == null) return NotFound();

            var result = await _category.Update(category);
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
    [Route("DeleteCategory/{Id}")]
    public async Task<ActionResult<bool>> DeleteCategory(long Id)
    {
        try
        {
            var result = await _category.Delete(Id);
            return result;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    #endregion
}