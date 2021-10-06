﻿using System;
using System.Collections.Generic;
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

            await qualificationRepository
                .Received(1)
                .GetAll();
        }

        [Fact]
        public async Task GetQualifications_Returns_Expected_List_From_Cache()
        {
            var qualificationRepository = Substitute.For<IQualificationRepository>();

            var cache = Substitute.For<IMemoryCache>();
            cache.TryGetValue(Arg.Any<string>(), out Arg.Any<IList<Qualification>>())
                .Returns(x =>
                {
                    if ((string)x[0] == CacheKeys.QualificationsKey)
                    {
                        x[1] = new QualificationBuilder().BuildList();
                        return true;
                    }

                    return false;
                });

            var service = new ProviderDataServiceBuilder()
                .Build(qualificationRepository: qualificationRepository,
                    cache: cache);

            var results = await service.GetQualifications();
            results.Should().NotBeNullOrEmpty();

            await qualificationRepository
                .DidNotReceive()
                .GetAll();
        }

        [Fact]
        public async Task GetRoutes_Returns_Expected_List()
        {
            var routes = new RouteBuilder()
                .BuildList()
                .ToList();

            var routeRepository = Substitute.For<IRouteRepository>();
            routeRepository.GetAll()
                .Returns(routes);

            var service = new ProviderDataServiceBuilder()
                .Build(routeRepository: routeRepository);

            var results = (await service.GetRoutes()).ToList();
            results.Should().NotBeNullOrEmpty();
            results.Count.Should().Be(routes.Count);

            await routeRepository
                .Received(1)
                .GetAll();
        }

        [Fact]
        public async Task GetRoutes_Returns_Expected_List_From_Cache()
        {
            var routeRepository = Substitute.For<IRouteRepository>();

            var cache = Substitute.For<IMemoryCache>();
            cache.TryGetValue(Arg.Any<string>(), out Arg.Any<IList<Route>>())
                .Returns(x =>
                {
                    if ((string)x[0] == CacheKeys.RoutesKey)
                    {
                        x[1] = new RouteBuilder().BuildList();
                        return true;
                    }

                    return false;
                });

            var service = new ProviderDataServiceBuilder()
                .Build(routeRepository: routeRepository,
                    cache: cache);

            var results = await service.GetRoutes();
            results.Should().NotBeNullOrEmpty();

            await routeRepository
                .DidNotReceive()
                .GetAll();
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
                .Returns(new ProviderSearchResultBuilder().BuildList());

            var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
            postcodeLookupService.GetPostcode(fromPostcodeLocation.Postcode)
                .Returns(fromPostcodeLocation);

            var service = new ProviderDataServiceBuilder().Build(
                postcodeLookupService: postcodeLookupService,
                providerRepository: providerRepository);

            var results = await service.FindProviders(fromPostcodeLocation.Postcode);
            results.Should().NotBeNull();
            results.Postcode.Should().Be(fromPostcodeLocation.Postcode);
            results.SearchResults.Should().NotBeNullOrEmpty();

            await postcodeLookupService
                .Received(1)
                .GetPostcode(fromPostcodeLocation.Postcode);
        }

        [Fact]
        public async Task FindProviders_Returns_Expected_List_For_Valid_Postcode_From_Cache()
        {
            var fromPostcodeLocation = new PostcodeLocationBuilder().BuildValidPostcodeLocation();

            var providerRepository = Substitute.For<IProviderRepository>();
            providerRepository.Search(
                    Arg.Is<PostcodeLocation>(p => p.Postcode == fromPostcodeLocation.Postcode),
                    Arg.Any<int?>(),
                    Arg.Any<int>(),
                    Arg.Any<int>())
                .Returns(new ProviderSearchResultBuilder().BuildList());

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
            results.Should().NotBeNull();
            results.Postcode.Should().Be(fromPostcodeLocation.Postcode);
            results.SearchResults.Should().NotBeNullOrEmpty();

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
                .Returns(new ProviderSearchResultBuilder().BuildList());

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

        [Fact]
        public async Task HasQualifications_Calls_Repository()
        {
            var qualificationRepository = Substitute.For<IQualificationRepository>();
            qualificationRepository.HasAny()
                .Returns(true);

            var service = new ProviderDataServiceBuilder()
                .Build(qualificationRepository: qualificationRepository);

            var result = await service.HasQualifications();

            result.Should().BeTrue();

            await qualificationRepository
                .Received(1)
                .HasAny();
        }

        [Fact]
        public async Task HasProviders_Calls_Repository()
        {
            var providerRepository = Substitute.For<IProviderRepository>();
            providerRepository.HasAny()
                .Returns(true);

            var service = new ProviderDataServiceBuilder()
                .Build(providerRepository: providerRepository);

            var result = await service.HasProviders();

            result.Should().BeTrue();

            await providerRepository
                .Received(1)
                .HasAny();
        }

    }
}
