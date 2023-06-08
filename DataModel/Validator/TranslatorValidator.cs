using DataModel.ViewModel;
using FluentValidation;

namespace DataModel.Validator;

public class TranslatorValidator : AbstractValidator<TranslatorVm>
{

    public TranslatorValidator()
    {
        RuleFor(p => p.TranslatorFirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Length(2, 25)
            .MinimumLength(3)
            //.Matches("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$") ******this can be for username
            .Must(IsValidName).WithMessage("{PropertyName} should be all letters.");

        RuleFor(p => p.TranslatorLastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Length(2, 25)
            .MinimumLength(3)
            .Must(IsValidName).WithMessage("{PropertyName} should be all letters.");

        RuleFor(p => p.Birthday)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            //.Matches(@"^((((31\/(0?[13578]|1[02]))|((29|30)\/(0?[1,3-9]|1[0-2])))\/(1[6-9]|[2-9]\d)?\d{2})|(29\/0?2\/(((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))|(0?[1-9]|1\d|2[0-8])\/((0?[1-9])|(1[0-2]))\/((1[6-9]|[2-9]\d)?\d{2})) (20|21|22|23|[0-1]?\d):[0-5]?\d$")
            .LessThan(p => DateTime.Now)
            .Must(BeAValidAge).WithMessage("Invalid {PropertyName}");


        RuleFor(p => p.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email Can not be Null")
            .EmailAddress()
            .Matches(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z")
            //.Matches(@"^[\w-_]+(\.[\w!#$%'*+\/=?\^`{|}]+)*@((([\-\w]+\.)+[a-zA-Z]{2,20})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$")
            .MinimumLength(10);

        RuleFor(p => p.Country)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Length(2, 25)
            .Must(IsValidName).WithMessage("{PropertyName} should be all letters.");

        RuleFor(p => p.Language)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Length(2, 25)
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
        var Countdigit = NumberOfDigits(dobYear);
        //var findd = dobYear.ToString().Count() >= 4;

        if (dobYear <= currentYear && Countdigit >= 4 && dobYear > 1930 && dobYear > (currentYear - 120))
        {
            return true;
        }

        return false;
    }

    private int GetNumberOfDigits(int num)
    {
        int count = 0;
        if (num != 0)
        {
            while (num > 0)
            {
                num /= 10;
                count++;
            }
            return count;
        }
        else
            return 1;
    }

    int NumberOfDigits(int num)
    {
        int digits = 0;
        while (num > 0)
        {
            ++digits;
            num = num / 10;
        }
        return digits;
    }
}
