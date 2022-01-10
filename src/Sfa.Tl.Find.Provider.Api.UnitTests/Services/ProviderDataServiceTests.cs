﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Services;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Services;

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
        var qualifications = new QualificationBuilder()
            .BuildList()
            .ToList();

        var qualificationRepository = Substitute.For<IQualificationRepository>();
        qualificationRepository.GetAll()
            .Returns(qualifications);

        var service = new ProviderDataServiceBuilder()
            .Build(qualificationRepository: qualificationRepository);

        var results = (await service.GetQualifications()).ToList();
        results.Should().NotBeNullOrEmpty();
        results.Count.Should().Be(qualifications.Count);

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
        var fromPostcodeLocation = PostcodeLocationBuilder.BuildValidPostcodeLocation();

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<PostcodeLocation>(p => p.Postcode == fromPostcodeLocation.Postcode),
                Arg.Any<int?>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<bool>())
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
        var fromPostcodeLocation = PostcodeLocationBuilder.BuildValidPostcodeLocation();

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<PostcodeLocation>(p => p.Postcode == fromPostcodeLocation.Postcode),
                Arg.Any<int?>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<bool>())
            .Returns(new ProviderSearchResultBuilder().BuildList());

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();

        var cache = Substitute.For<IMemoryCache>();
        cache.TryGetValue(Arg.Any<string>(), out Arg.Any<PostcodeLocation>())
            .Returns(x =>
            {
                if (((string)x[0]).Contains(fromPostcodeLocation.Postcode.Replace(" ", "")))
                {
                    x[1] = PostcodeLocationBuilder.BuildPostcodeLocation(fromPostcodeLocation.Postcode);
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
    public async Task FindProviders_Returns_Expected_List_For_Valid_Outcode()
    {
        var fromPostcodeLocation = PostcodeLocationBuilder.BuildValidOutwardPostcodeLocation();

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<PostcodeLocation>(p => p.Postcode == fromPostcodeLocation.Postcode),
                Arg.Any<int?>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<bool>())
            .Returns(new ProviderSearchResultBuilder().BuildList());

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetOutcode(fromPostcodeLocation.Postcode)
            .Returns(fromPostcodeLocation);

        var service = new ProviderDataServiceBuilder().Build(
            postcodeLookupService: postcodeLookupService,
            providerRepository: providerRepository);

        var results = await service.FindProviders(fromPostcodeLocation.Postcode);
        results.Should().NotBeNull();
        results.Postcode.Should().Be(fromPostcodeLocation.Postcode);
        results.SearchResults.Should().NotBeNullOrEmpty();

        await postcodeLookupService
            .DidNotReceive()
            .GetPostcode(fromPostcodeLocation.Postcode);
        await postcodeLookupService
            .Received(1)
            .GetOutcode(fromPostcodeLocation.Postcode);
    }

    [Fact]
    public async Task FindProviders_Returns_Expected_Error_Details_For_Bad_Postcode()
    {
        var fromPostcodeLocation = PostcodeLocationBuilder.BuildInvalidPostcodeLocation();

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<PostcodeLocation>(p => p.Postcode == fromPostcodeLocation.Postcode),
                Arg.Any<int?>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<bool>())
            .Returns(new ProviderSearchResultBuilder().BuildList());

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(fromPostcodeLocation.Postcode)
            .Returns((PostcodeLocation)null);

        var service = new ProviderDataServiceBuilder().Build(
            postcodeLookupService: postcodeLookupService,
            providerRepository: providerRepository);

        var results = await service.FindProviders(fromPostcodeLocation.Postcode);
        results.Should().NotBeNull();
        results.Error.Should().Be("The postcode was not found");
        results.Postcode.Should().BeNull();
        results.SearchResults.Should().BeNull();

        await postcodeLookupService
            .Received(1)
            .GetPostcode(fromPostcodeLocation.Postcode);
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

    [Fact]
    public async Task LoadAdditionalProviderData_Calls_Repository_To_Save_Data()
    {
        var providerRepository = Substitute.For<IProviderRepository>();

        var service = new ProviderDataServiceBuilder()
            .Build(providerRepository: providerRepository);

        await service.LoadAdditionalProviderData();

        await providerRepository
            .Received(1)
            .Save(Arg.Any<IList<Models.Provider>>(),
                Arg.Is<bool>(b => b));
    }

    [Fact]
    public async Task LoadAdditionalProviderData_Calls_Repository_To_Save_Data_With_Additional_Data_Flag_Set()
    {
        var providerRepository = Substitute.For<IProviderRepository>();

        IList<Models.Provider> receivedProviders = null;

        await providerRepository
            .Save(Arg.Do<IList<Models.Provider>>(
                x => receivedProviders = x),
                Arg.Any<bool>());

        var service = new ProviderDataServiceBuilder()
            .Build(providerRepository: providerRepository);

        await service.LoadAdditionalProviderData();

        receivedProviders.Should().NotBeNullOrEmpty();

        foreach (var provider in receivedProviders)
        {
            provider.IsAdditionalData.Should().BeTrue();
            foreach (var location in provider.Locations)
            {
                location.IsAdditionalData.Should().BeTrue();
            }
        }
    }

    [Fact]
    public async Task LoadAdditionalProviderData_Loads_Expected_Provider()
    {
        var providerRepository = Substitute.For<IProviderRepository>();

        IList<Models.Provider> receivedProviders = null;

        await providerRepository
            .Save(Arg.Do<IList<Models.Provider>>(
                x => receivedProviders = x),
                Arg.Any<bool>());

        var service = new ProviderDataServiceBuilder()
            .Build(providerRepository: providerRepository);

        await service.LoadAdditionalProviderData();

        receivedProviders.Should().NotBeNullOrEmpty();

        var provider = receivedProviders.SingleOrDefault(p => p.UkPrn == 10035123);
        provider.Should().NotBeNull();

        provider!.Name.Should().Be("BIDDULPH HIGH SCHOOL");
        provider.Postcode.Should().Be("ST8 7AR");
        provider.Website.Should().Be("https://biddulphhigh.co.uk/");
        provider.Email.Should().Be("office@biddulphhigh.co.uk");
        provider.Telephone.Should().Be("01782 523977");
        provider.Town.Should().Be("STOKE-ON-TRENT");

        provider.IsAdditionalData.Should().BeTrue();

        provider.Locations.Should().NotBeNull();
        provider.Locations.Count.Should().Be(1);

        var location = provider.Locations.First();

        location.Postcode.Should().Be("ST8 7AR");
        location.Town.Should().Be("STOKE-ON-TRENT");
        location.Latitude.Should().Be(53.105857);
        location.Longitude.Should().Be(-2.171092);
        location.Website.Should().Be("https://biddulphhigh.co.uk/");
        location.Email.Should().Be("office@biddulphhigh.co.uk");
        location.Telephone.Should().Be("01782 523977");
        location.DeliveryYears.Should().NotBeNull();
        location.DeliveryYears.Count.Should().Be(1);

        var deliveryYear = location.DeliveryYears.First();

        deliveryYear.Year.Should().Be(2022);
        deliveryYear.Qualifications.Should().NotBeNull();
        deliveryYear.Qualifications.Count.Should().Be(1);

        var qualification = deliveryYear.Qualifications.First();
        qualification.Id.Should().Be(41);
    }
}