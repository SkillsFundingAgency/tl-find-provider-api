using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Json;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;
public class DfeSignInApiServiceTests
{
    private const string TestOrganisationId = "E100D38D-6385-4C15-809D-832C8F7F48E1";
    private const string TestUserId = "D5942B2A-36BD-4D2C-9522-52C2DCC2FE04";
    private const int TestUkPrn = 01234567;
    private const int TestUrn = 123456;
    private const string TestRoleName = "Standard";

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

    //[Fact]
    //public async Task GetDfeSignInUserInfo_Returns_Expected_Value()
    //{
    //    var serviceBuilder = new DfeSignInApiServiceBuilder();
    //    var settings = new SettingsBuilder().BuildDfeSignInSettings();

    //    var responses = new Dictionary<string, string>
    //    {
    //        {
    //            serviceBuilder.BuildGetOrganisationsUriFragment(TestUserId),
    //            DfeSignInApiJsonBuilder.BuildOrganisationsResponse()
    //        },
    //        {
    //            //$"/services/{settings.ClientId}/organisations/{TestOrganisationId}/users/{TestUserId}",
    //            serviceBuilder.BuildGetUserUriFragment(TestOrganisationId, TestUserId, settings),
    //            DfeSignInApiJsonBuilder.BuildUserResponse()
    //        }
    //    };
    //    var service = serviceBuilder.Build(responses, settings);

    //    var result = await service
    //        .GetDfeSignInUserInfo(TestOrganisationId, TestUserId);

    //    result.Should().NotBeNull();
    //    result.UserId.Should().Be(TestUserId);
    //    result.UkPrn.Should().Be(TestUkPrn);
    //    result.HasAccessToService = true;
    //    //result.Urn.Should().Be(TestUrn);
    //    result.Roles.Should().NotBeNullOrEmpty();
    //    result.Roles.Count().Should().Be(1);
    //    result.Roles.First().Name.Should().Be(TestRoleName);
    //}
    [Fact]
    public async Task GetDfeSignInInfo_Returns_Expected_Value()
    {
        var serviceBuilder = new DfeSignInApiServiceBuilder();
        var settings = new SettingsBuilder().BuildDfeSignInSettings();

        var responses = new Dictionary<string, string>
        {
            {
                serviceBuilder.BuildGetOrganisationsUriFragment(TestUserId),
                DfeSignInApiJsonBuilder.BuildOrganisationsResponse()
            },
            {
                //$"/services/{settings.ClientId}/organisations/{TestOrganisationId}/users/{TestUserId}",
                serviceBuilder.BuildGetUserUriFragment(TestOrganisationId, TestUserId, settings),
                DfeSignInApiJsonBuilder.BuildUserResponse()
            }
        };
        var service = serviceBuilder.Build(responses, settings);

        var (organisation, user) = await service
            .GetDfeSignInInfo(TestOrganisationId, TestUserId);

        organisation.Should().NotBeNull();
        organisation.UkPrn.Should().Be(TestUkPrn);
        organisation.Urn.Should().Be(TestUrn);

        user.Should().NotBeNull();
        user.UserId.Should().Be(TestUserId);
        user.HasAccessToService = true;
        user.Roles.Should().NotBeNullOrEmpty();
        user.Roles.Count().Should().Be(1);
        user.Roles.First().Name.Should().Be(TestRoleName);
    }
}
