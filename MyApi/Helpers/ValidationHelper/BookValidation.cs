using DataModel.Validator;
using DataModel.Validator.ResponseModel;
using DataModel.ViewModel;
using FluentValidation.Results;

namespace MyApi.Helpers.ValidationHelper;

public class BookValidation
{
    public static (List<string> ValidationMessages, bool IsValid) ApplyBookValidator(AddBookViewModel bookVm)
    {
        //var authorVm = new AuthorViewModel();

        BookValidator validator = new BookValidator();
        List<string> ValidationMessages = new List<string>();

        var validationResult = validator.Validate(bookVm);
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
    }
}
