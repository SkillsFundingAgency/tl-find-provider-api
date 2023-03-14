using Sfa.Tl.Find.Provider.Api.Validators;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Validators;
public class EmployerInterestInputModelValidatorTests
{
    [Fact]
    public void EmployerInterestInputModelValidator_Validates()
    {
        var target = new EmployerInterestInputModel
        {
            Locations = null
        };

        var validator = new EmployerInterestInputModelValidator();
        var result = validator.Validate(target);

        result.Should().NotBeNull();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should()
            .Contain(x =>
                x.PropertyName == "Locations" &&
                x.ErrorMessage == "One or more locations must be provided.");
    }
}
