using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Models.Exceptions;
using Sfa.Tl.Find.Provider.Api.Services;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Services
{
    public class ProviderDataServiceTests
    {
        private const string TestPostcode = "AB1 2XY";
        private const string InvalidPostcode = "CV1 3WT";

        [Fact]
        public void Constructor_Guards_Against_NullParameters()
        {
            typeof(ProviderDataService)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void Constructor_Guards_Against_BadParameters()
        {
            typeof(ProviderDataService)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public async Task GetQualifications_Returns_Expected_List()
        {
            var qualificationRepository = Substitute.For<IQualificationRepository>();
            qualificationRepository.GetAll()
                .Returns(new QualificationBuilder().BuildList().AsQueryable());

            var service = new ProviderDataServiceBuilder()
                .Build(qualificationRepository: qualificationRepository);

            var results = await service.GetQualifications();
            results.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task FindProviders_Returns_Expected_List_For_Valid_Postcode()
        {
            var providerRepository = Substitute.For<IProviderRepository>();
            providerRepository.Search(
                    TestPostcode,
                    Arg.Any<int?>(),
                    Arg.Any<int>(),
                    Arg.Any<int>())
                .Returns(new ProviderBuilder().BuildList().AsQueryable());

            var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
            postcodeLookupService.GetPostcode(TestPostcode)
                .Returns(new PostcodeLocationBuilder().BuildPostcodeLocation(TestPostcode));

            var service = new ProviderDataServiceBuilder().Build(
                postcodeLookupService: postcodeLookupService,
                providerRepository: providerRepository);

            var results = await service.FindProviders(TestPostcode);
            results.Should().NotBeNullOrEmpty();

            await postcodeLookupService.Received(1).GetPostcode(TestPostcode);
        }

        [Fact]
        public async Task FindProviders_Returns_Expected_List_For_Valid_Postcode_From_Cache()
        {
            var providerRepository = Substitute.For<IProviderRepository>();
            providerRepository.Search(
                    TestPostcode,
                    Arg.Any<int?>(),
                    Arg.Any<int>(),
                    Arg.Any<int>())
                .Returns(new ProviderBuilder().BuildList().AsQueryable());

            var postcodeLookupService = Substitute.For<IPostcodeLookupService>();

            var cache = Substitute.For<IMemoryCache>();
            cache.TryGetValue(Arg.Any<string>(), out Arg.Any<PostcodeLocation>())
                .Returns(x =>
                {
                    if (((string)x[0]).Contains(TestPostcode.Replace(" ", "")))
                    {
                        x[1] = new PostcodeLocationBuilder().BuildPostcodeLocation(TestPostcode);
                        return true;
                    }

                    return false;
                });

            var service = new ProviderDataServiceBuilder().Build(
                postcodeLookupService: postcodeLookupService,
                providerRepository: providerRepository,
                cache: cache);

            var results = await service.FindProviders(TestPostcode);
            results.Should().NotBeNullOrEmpty();

            await postcodeLookupService.DidNotReceive().GetPostcode(TestPostcode);
        }

        [Fact]
        public async Task FindProviders_Throws_Exception_For_Bad_Postcode()
        {
            var providerRepository = Substitute.For<IProviderRepository>();
            providerRepository.Search(TestPostcode,
                    Arg.Any<int?>(),
                    Arg.Any<int>(),
                    Arg.Any<int>())
                .Returns(new ProviderBuilder().BuildList().AsQueryable());

            var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
            postcodeLookupService.GetPostcode(TestPostcode)
                .Returns((PostcodeLocation)null);

            var service = new ProviderDataServiceBuilder().Build(
                 postcodeLookupService: postcodeLookupService,
                 providerRepository: providerRepository);

            Func<Task> target = async () => await service.FindProviders(InvalidPostcode);

            await target.Should()
                .ThrowAsync<PostcodeNotFoundException>()
                .WithMessage($"*{InvalidPostcode}*");
        }
    }
}
