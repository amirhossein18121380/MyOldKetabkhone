using DataModel.Validator;
using DataModel.Validator.ResponseModel;
using DataModel.ViewModel;
using FluentValidation.Results;

namespace MyApi.Helpers.ValidationHelper;

public static class TranslatorValidation
{
    #region FluentValidation For Translator

    public static (List<string> ValidationMessages, bool IsValid) ApplyTranslatorValidator(TranslatorVm translatorVm)
    {
        //var authorVm = new AuthorViewModel();

        TranslatorValidator validator = new TranslatorValidator();
        List<string> ValidationMessages = new List<string>();

        var validationResult = validator.Validate(translatorVm);
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

    //public static List<string> ApplyTranslatorValidator(TranslatorVm translatorVm)
    //{
    //    //var authorVm = new AuthorViewModel();

    //    TranslatorValidator validator = new TranslatorValidator();
    //    List<string> ValidationMessages = new List<string>();

    //    var validationResult = validator.Validate(translatorVm);
    //    var response = new ResponseModel();

    //    response.IsValid = false;
    //    foreach (ValidationFailure failure in validationResult.Errors)
    //    {
    //        ValidationMessages.Add(failure.ErrorMessage);
    //    }
    //    response.ValidationMessages = ValidationMessages;

    //    return ValidationMessages;
    //}
    #endregion
}