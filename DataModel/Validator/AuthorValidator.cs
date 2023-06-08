
using DataModel.ViewModel;
using FluentValidation;

namespace DataModel.Validator;

public class AuthorValidator : AbstractValidator<AuthorViewModel>
{

    public AuthorValidator()
    {
        RuleFor(p => p.AuthorFirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Length(2, 25)
            .MinimumLength(3)
            .Must(IsValidName).WithMessage("{PropertyName} should be all letters.");

        RuleFor(p => p.AuthorLastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Length(2, 25)
            .MinimumLength(3)
            .Must(IsValidName).WithMessage("{PropertyName} should be all letters.");

        RuleFor(p => p.Birthday)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Must(BeAValidAge).WithMessage("Invalid {PropertyName}");

        //RuleFor(p => p.Email)
        //    .Cascade(CascadeMode.Stop)
        //    .NotEmpty().WithMessage("Email Can not be Null")
        //    .EmailAddress()
        //    .MinimumLength(10);

        RuleFor(p => p.Country)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Length(2, 25)
            .Must(IsValidName).WithMessage("{PropertyName} should be all letters.");

        RuleFor(p => p.Language)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Must(IsValidName).WithMessage("{PropertyName} should be all letters.");

        //RuleFor(p => p.Age)
        //    .Cascade(CascadeMode.StopOnFirstFailure)
        //    .NotNull()
        //    .NotEmpty()
        //    .WithErrorCode("1002")
        //    .LessThan(150)
        //    .GreaterThan(1); 
            //.Must(IsValidInt).WithMessage("{PropertyName} should be all.");

        RuleFor(p => p.Bio)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Length(2, 25)
            .Must(IsValidName).WithMessage("{PropertyName} should be all letters.");
    }


    private bool IsValidName(string name)
    {
        return name.All(Char.IsLetter);
    }

    //private bool IsValidInt(string age)
    //{
    //    return age.All(Char.IsLetterOrDigit);
    //}

    protected bool BeAValidAge(DateTime date)
    {
        int currentYear = DateTime.Now.Year;
        int dobYear = date.Year;

        if (dobYear <= currentYear && dobYear > (currentYear - 120))
        {
            return true;
        }

        return false;
    }
}

