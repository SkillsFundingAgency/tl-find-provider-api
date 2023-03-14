using FluentValidation;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Api.Validators;

public class EmployerInterestInputModelValidator
: AbstractValidator<EmployerInterestInputModel>
{
    public EmployerInterestInputModelValidator()
    {
        RuleFor(x => x.Locations)
            .NotEmpty()
            .WithMessage("One or more locations must be provided.");
    }
}
