using DataAccess.Interface;
using DataModel.Models;
using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookSubjectController : ControllerBase
{
    #region Constructor
    private readonly ISubjectDal _subject;

    public BookSubjectController(ISubjectDal subject)
    {
        _subject = subject;
    }
    #endregion

    #region Fetch
    [HttpGet]
    [Route("GetAllSubjects")]
    public async Task<ActionResult<List<Subjects>>> GetAllSubjects()
    {
        try
        {
            var companies = await _subject.GetAll();
            return Ok(companies);
        }
        catch (Exception ex)
        {
            //log error
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("GetSubjectById/{id}")]
    public async Task<ActionResult<Subjects>> GetSubjectById(long id)
    {
        try
        {
            var results = await _subject.GetById(id);
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
    [Route("AddSubject")]
    public async Task<ActionResult<bool>> AddSubject(Subjects category)
    {
        try
        {
            var result = await _subject.Insert(category);

            return result > 0;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }


    #endregion

    #region Update

    [HttpPost]
    [Route("UpdateSubject")]
    public async Task<ActionResult<bool>> UpdateSubject(Subjects category)
    {
        try
        {
            //var tes = await data.GetStudentById(student.Id);
            // if(tes == null) return NotFound();

            var result = await _subject.Update(category);
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

    [HttpPost]
    [Route("DeleteSubject/{Id}")]
    public async Task<ActionResult<bool>> DeleteSubject(long Id)
    {
        try
        {
            var result = await _subject.Delete(Id);
            return result;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    #endregion
}
