using System;
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
                .Returns(new QualificationBuilder().BuildList());

            var service = new ProviderDataServiceBuilder()
                .Build(qualificationRepository: qualificationRepository);

            var results = await service.GetQualifications();
            results.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task FindProviders_Returns_Expected_List_For_Valid_Postcode()
        {
            var fromPostcodeLocation = new PostcodeLocationBuilder().BuildValidPostcodeLocation();

            var providerRepository = Substitute.For<IProviderRepository>();
            providerRepository.Search(
                    Arg.Is<PostcodeLocation>(p => p.Postcode == fromPostcodeLocation.Postcode),
                    Arg.Any<int?>(),
                    Arg.Any<int>(),
                    Arg.Any<int>())
                .Returns(new ProviderBuilder().BuildList());

            var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
            postcodeLookupService.GetPostcode(fromPostcodeLocation.Postcode)
                .Returns(fromPostcodeLocation);

            var service = new ProviderDataServiceBuilder().Build(
                postcodeLookupService: postcodeLookupService,
                providerRepository: providerRepository);

            var results = await service.FindProviders(fromPostcodeLocation.Postcode);
            results.Should().NotBeNullOrEmpty();

            await postcodeLookupService.Received(1).GetPostcode(fromPostcodeLocation.Postcode);
        }

        [Fact]
        public async Task FindProviders_Returns_Expected_List_For_Valid_Postcode_From_Cache()
        {
            var fromPostcodeLocation = new PostcodeLocationBuilder().BuildValidPostcodeLocation();

            var providerRepository = Substitute.For<IProviderRepository>();
            providerRepository.Search(
                    Arg.Is<PostcodeLocation>(p => p.Postcode ==  fromPostcodeLocation.Postcode),
                    Arg.Any<int?>(),
                    Arg.Any<int>(),
                    Arg.Any<int>())
                .Returns(new ProviderBuilder().BuildList());

            var postcodeLookupService = Substitute.For<IPostcodeLookupService>();

            var cache = Substitute.For<IMemoryCache>();
            cache.TryGetValue(Arg.Any<string>(), out Arg.Any<PostcodeLocation>())
                .Returns(x =>
                {
                    if (((string)x[0]).Contains(fromPostcodeLocation.Postcode.Replace(" ", "")))
                    {
                        x[1] = new PostcodeLocationBuilder().BuildPostcodeLocation(fromPostcodeLocation.Postcode);
                        return true;
                    }

                    return false;
                });

            var service = new ProviderDataServiceBuilder().Build(
                postcodeLookupService: postcodeLookupService,
                providerRepository: providerRepository,
                cache: cache);

            var results = await service.FindProviders(fromPostcodeLocation.Postcode);
            results.Should().NotBeNullOrEmpty();

            await postcodeLookupService
                .DidNotReceive()
                .GetPostcode(Arg.Any<string>());
        }

        [Fact]
        public async Task FindProviders_Throws_Exception_For_Bad_Postcode()
        {
            var fromPostcodeLocation = new PostcodeLocationBuilder().BuildInvalidPostcodeLocation();

            var providerRepository = Substitute.For<IProviderRepository>();
            providerRepository.Search(
                    Arg.Is<PostcodeLocation>(p => p.Postcode == fromPostcodeLocation.Postcode),
                    Arg.Any<int?>(),
                    Arg.Any<int>(),
                    Arg.Any<int>())
                .Returns(new ProviderBuilder().BuildList());

            var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
            postcodeLookupService.GetPostcode(fromPostcodeLocation.Postcode)
                .Returns((PostcodeLocation)null);

            var service = new ProviderDataServiceBuilder().Build(
                 postcodeLookupService: postcodeLookupService,
                 providerRepository: providerRepository);

            Func<Task> target = async () => await service.FindProviders(fromPostcodeLocation.Postcode);

            await target.Should()
                .ThrowAsync<PostcodeNotFoundException>()
                .WithMessage($"*{fromPostcodeLocation.Postcode}*");
        }
    }
}
