using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel.ViewModel;
using FluentValidation;

namespace DataModel.Validator;

public class BookValidator : AbstractValidator<AddBookViewModel>
{
    public BookValidator()
    {
        RuleFor(p => p.BookName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Length(2, 25);

        //RuleFor(p => p.Author)
        //    .Cascade(CascadeMode.Stop)
        //    .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
        //    .Length(2, 25);

        //RuleFor(p => p.Translator)
        //    .Cascade(CascadeMode.Stop)
        //    .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
        //    .Length(2, 25);

        RuleFor(p => p.Publisher)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Length(2, 25);

        RuleFor(p => p.YearOfPublication)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Must(BeAValidYear);


        RuleFor(p => p.NumberOfPages)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .GreaterThan(1);

        RuleFor(p => p.Language)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Must(IsValidName).WithMessage("{PropertyName} should be all letters.");

        RuleFor(p => p.ISBN)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
            .Must(IsValidNumber).WithMessage("{ PropertyName} should be all digits");

        //RuleFor(p => p.BookSubject)
        //    .Cascade(CascadeMode.Stop)
        //    .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
        //    .Length(2, 25);

        RuleFor(p => p.ElectronicVersionPrice)
           .Cascade(CascadeMode.Stop)
           .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
           .Must(IsValidDesimal).WithMessage("{ PropertyName} should be all digits");

        //RuleFor(p => p.BookPictureName)
        //   .Cascade(CascadeMode.Stop)
        //   .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!");

        //RuleFor(p => p.BookPictureId)
        //   .Cascade(CascadeMode.Stop)
        //   .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
        //   .Must(IsValidLong).WithMessage("{PropertyName} should be all digits.");
    }

    protected bool BeAValidYear(DateTime date)
    {
        int currentYear = DateTime.Now.Year;
        int dobYear = date.Year;

        if (dobYear <= currentYear && dobYear > (currentYear - 120))
        {
            return true;
        }

        return false;
    }


    private bool IsValidName(string name)
    {
        return name.All(Char.IsLetter);
    }

    private bool IsValidLong(long picid)
    {
        var lo = Convert.ToString(picid);
        return lo.All(Char.IsDigit);
    }

    private bool IsValidNumber(long isbn)
    {
        var ToInt = Convert.ToInt16(isbn);
        var ToStr = ToInt.ToString();
        return ToStr.All(Char.IsDigit);
    }

    private bool IsValidDesimal(decimal money)
    {
        var Tostr = Convert.ToString(money);
        return Tostr.All(Char.IsDigit);
    }

}