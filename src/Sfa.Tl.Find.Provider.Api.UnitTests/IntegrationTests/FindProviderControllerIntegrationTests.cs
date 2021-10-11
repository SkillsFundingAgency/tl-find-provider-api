using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.IntegrationTests
{
    public class FindProviderControllerIntegrationTests : IClassFixture<TestServerFactory<FakeStartup>>
    {
        private readonly TestServerFactory<FakeStartup> _fixture;

        public FindProviderControllerIntegrationTests(TestServerFactory<FakeStartup> fixture)
        {
            _fixture = fixture;
        }

        [Fact(Skip = "TODO: Make this test set HMAC header")]
        public async Task GetProviders_Returns_OK_Result_For_Valid_Url()
        {
            var response = await _fixture
                .CreateClient()
                .GetAsync("/api/v1/findproviders/providers?postcode=CV1+2WT&qualificationId=40&page=0&pageSize=5");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact(Skip = "TODO: Make this test set HMAC header")]
        public async Task GetProviders_Returns_Bad_Request_Result_For_Missing_Postcode()
        {
            var response = await _fixture
                .CreateClient()
                .GetAsync("/api/v1/findproviders/providers");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact(Skip = "TODO: Make this test set HMAC header")]
        public async Task GetProviders_Returns_Bad_Request_Result_For_Zero_PageSize()
        {
            var response = await _fixture
                .CreateClient()
                .GetAsync("/api/v1/findproviders/providers?postcode=CV1+2WT&page=0&pageSize=0");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await response.Content.ValidateProblemDetails(
                ("pageSize", "The pageSize field must be at least one."));
        }

        [Fact(Skip = "TODO: Make this test set HMAC header")]
        public async Task GetProviders_Returns_Bad_Request_Result_For_Negative_Page()
        {
            var response = await _fixture
                .CreateClient()
                .GetAsync("/api/v1/findproviders/providers?postcode=CV1+2WT&page=-1&pageSize=5");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await response.Content.ValidateProblemDetails(
                ("page", "The page field must be zero or greater."));
        }

        [Fact(Skip = "TODO: Make this test set HMAC header")]
        public async Task GetProviders_Returns_Bad_Request_Result_With_All_Errors()
        {
            var response = await _fixture
                .CreateClient()
                .GetAsync("/api/v1/findproviders/providers?qualificationId=40&page=-1&pageSize=0");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await response.Content.ValidateProblemDetails(
                ("postcode", "The postcode field is required."),
                ("pageSize", "The pageSize field must be at least one."),
                ("page", "The page field must be zero or greater."));

        }

        [Fact(Skip = "TODO: Make this test set HMAC header")]
        public async Task GetQualifications_Returns_OK_Result_For_Valid_Url()
        {
            var response = await _fixture
                .CreateClient()
                .GetAsync("/api/v1/findproviders/qualifications");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        //Not_Found in initial version, because method is hidden
        public async Task GetRoutes_Returns_Not_Found_Result_For_Valid_Url()
        {
            var response = await _fixture
                .CreateClient()
                .GetAsync("/api/v1/findproviders/routes");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
