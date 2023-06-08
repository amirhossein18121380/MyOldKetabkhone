using Common.Helper;
using DataModel.Validator;
using DataModel.Validator.ResponseModel;
using DataModel.ViewModel;
using FluentValidation;
using FluentValidation.Results;

namespace MyApi.Helpers.ValidationHelper;

public static class AuthorValidation
{
    #region FluentValidation For Author
    public static (List<string> ValidationMessages, bool IsValid) ApplyAuthorValidator(AuthorViewModel authorVm)
    {
        //var authorVm = new AuthorViewModel();

        AuthorValidator validator = new AuthorValidator();
        List<string> ValidationMessages = new List<string>();

        var validationResult = validator.Validate(authorVm);
        var response = new ResponseModel();
        if (!validationResult.IsValid)
        {
            response.IsValid = false;
            foreach (ValidationFailure failure in validationResult.Errors)
            {
                ValidationMessages.Add(failure.ErrorMessage);
            }
            response.ValidationMessages = ValidationMessages;
            //if (validationResult.Errors.Count == 0)
            //{

            //}
            return (response.ValidationMessages, response.IsValid);
        }
        return (response.ValidationMessages, response.IsValid);

        //**********this below was in controller

        //AuthorValidator validator = new AuthorValidator();
        //List<string> ValidationMessages = new List<string>();

        //var validationResult = validator.Validate(authorVm);
        //var response = new ResponseModel();
        //if (!validationResult.IsValid)
        //{
        //    response.IsValid = false;
        //    foreach (ValidationFailure failure in validationResult.Errors)
        //    {
        //        ValidationMessages.Add(failure.ErrorMessage);
        //    }
        //    response.ValidationMessages = ValidationMessages;
        //    return BadRequest(response.ValidationMessages);
        //}           

        //return Ok(response);
    }
    #endregion
}