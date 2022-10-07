using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;
public class DfeSignInApiServiceTests
{
    private const string TestOrganisationId = "1582317";
    private const string TestUserId = "92264E39-0708-47BA-9D07-E7A4D31E8636";
    private const int TestUkPrn = 01234567;

    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(DfeSignInApiService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(DfeSignInApiService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task GetDfeSignInUserInfo_Returns_Expected_Value()
    {
        var service = new DfeSignInApiServiceBuilder()
            .Build();

        //TODO: Build out http details for test - need sample data first
        //var result = await service
        //    .GetDfeSignInUserInfo(TestOrganisationId, TestUserId);

        //result.Should().NotBeNull();
        //result.UserId.Should().Be(TestUserId);
        //result.UkPrn.Should().Be(TestUkPrn);
    }
}
