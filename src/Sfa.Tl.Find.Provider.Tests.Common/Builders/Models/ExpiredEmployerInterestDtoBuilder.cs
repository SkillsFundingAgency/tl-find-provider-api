using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class ExpiredEmployerInterestDtoBuilder
{
    public IEnumerable<ExpiredEmployerInterestDto> BuildList() =>
        new List<ExpiredEmployerInterestDto>
        {
            new()
            {
                Id = 1,
                UniqueId = Guid.Parse("fac9cd89-41eb-4f57-9dea-3f11554f3a04"),
                OrganisationName = "Test Organisation 1",
                Postcode = "CV1 2WT",
                Email = "test1@test.com"
            },
            new ()
            {
                Id = 2,
                UniqueId = Guid.Parse("5631dfb2-edfa-4d09-8307-ae13046f43e3"),
                OrganisationName = "Test Organisation 2",
                Postcode = "CV1 3GT",
                Email = "test2@test.com"
            }
        };

    public IList<ExpiredEmployerInterestDto> BuildList(int numberToTake) =>
        BuildList().Take(numberToTake).ToList();

    public ExpiredEmployerInterestDto Build() =>
        BuildList().First();
}