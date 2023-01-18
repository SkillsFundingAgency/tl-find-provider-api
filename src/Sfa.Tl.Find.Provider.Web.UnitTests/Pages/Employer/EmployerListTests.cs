using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Employer;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Employer;
public class EmployerListTests
{
    private const string CustomPostcodeKey = "Input.CustomPostcode";
    private const string InvalidFormatPostcode = "CVX XXX";
    private const string InvalidPostcode = "CV1 2XX";
    private const string ValidPostcode = "CV1 2WT";

    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(EmployerListModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task EmployerListModel_OnGet_Sets_ExpectedProperties()
    {
        const int retentionDays = 84;
        const int expectedRetentionWeeks = 12;

        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            retentionDays: retentionDays);

        var employerListModel = new EmployerListModelBuilder()
            .Build(employerInterestSettings: settings);

        await employerListModel.OnGet();

        employerListModel.UkPrn
            .Should()
            .Be(PageContextBuilder.DefaultUkPrn);

        employerListModel.SearchRadius.Should().Be(settings.SearchRadius);
        employerListModel.EmployerInterestRetentionDays.Should().Be(retentionDays);
        employerListModel.EmployerInterestRetentionWeeks.Should().Be(expectedRetentionWeeks);

        employerListModel.SelectedPostcodeHasFilters.Should().BeFalse();
    }

    [Fact]
    public async Task EmployerListModel_OnGet_Populates_EmployerInterest_List_For_Administrator()
    {
        var employerInterestSummary = new EmployerInterestSummaryBuilder()
            .BuildList()
            .ToList();

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .GetSummaryList()
            .Returns(employerInterestSummary);

        var employerListModel = new EmployerListModelBuilder()
            .Build(employerInterestService,
                isAdministrator: true);

        await employerListModel.OnGet();

        employerListModel.EmployerInterestList
            .Should()
            .BeEquivalentTo(employerInterestSummary);

        employerListModel.ZeroResultsFound.Should().NotBeNull();
        employerListModel.ZeroResultsFound!.Value.Should().BeFalse();
        employerListModel.SelectedPostcodeHasFilters.Should().BeFalse();
    }

    [Fact]
    public async Task EmployerListModel_OnGet_Does_Not_Populate_EmployerInterest_List_When_Session_Has_No_Postcode()
    {
        var employerInterestSummary = new EmployerInterestSummaryBuilder()
            .BuildList()
            .ToList();

        var sessionService = Substitute.For<ISessionService>();
        sessionService
            .Get<LocationPostcode>(EmployerListModel.SessionKeyPostcodeLocation)
            .Returns((LocationPostcode)null!);

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .FindEmployerInterest(ValidPostcode)
            .Returns((employerInterestSummary, 2));

        var employerListModel = new EmployerListModelBuilder()
            .Build(employerInterestService,
                sessionService: sessionService);

        await employerListModel.OnGet();

        employerListModel.EmployerInterestList
            .Should()
            .BeNullOrEmpty();

        employerListModel.ZeroResultsFound.Should().BeNull();
        employerListModel.SelectedPostcodeHasFilters.Should().BeFalse();
    }

    [Fact]
    public async Task EmployerListModel_OnGet_Populates_EmployerInterest_List_When_Session_Has_Postcode()
    {
        var locationPostcode = new LocationPostcodeBuilder()
            .Build();

        var employerInterestSummary = new EmployerInterestSummaryBuilder()
            .BuildList()
            .ToList();

        var sessionService = Substitute.For<ISessionService>();
        sessionService
            .Get<LocationPostcode>(EmployerListModel.SessionKeyPostcodeLocation)
            .Returns(locationPostcode);

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .FindEmployerInterest(locationPostcode.Latitude, locationPostcode.Longitude)
            .Returns((employerInterestSummary, 2));

        var employerListModel = new EmployerListModelBuilder()
            .Build(employerInterestService,
                sessionService: sessionService);

        await employerListModel.OnGet();

        employerListModel.EmployerInterestList
            .Should()
            .BeEquivalentTo(employerInterestSummary);

        employerListModel.ZeroResultsFound.Should().NotBeNull();
        employerListModel.ZeroResultsFound!.Value.Should().BeFalse();
        employerListModel.SelectedPostcodeHasFilters.Should().BeFalse();
    }

    [Fact]
    public async Task EmployerListModel_OnGet_Populates_EmployerInterest_List_For_Known_Location()
    {
        var locationPostcode = new LocationPostcodeBuilder()
            .Build();

        var employerInterestSummary = new EmployerInterestSummaryBuilder()
            .BuildList()
            .ToList();

        var locationPostcodes = new LocationPostcodeBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetLocationPostcodes(PageContextBuilder.DefaultUkPrn)
            .Returns(locationPostcodes);

        var sessionService = Substitute.For<ISessionService>();
        sessionService
            .Get<LocationPostcode>(EmployerListModel.SessionKeyPostcodeLocation)
            .Returns(locationPostcode);

        var geoLocation = GeoLocationBuilder.BuildGeoLocation(ValidPostcode);
        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(ValidPostcode)
            .Returns(geoLocation);

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .FindEmployerInterest(locationPostcode.Id!.Value)
            .Returns((employerInterestSummary, 2, true));

        var employerListModel = new EmployerListModelBuilder()
            .Build(employerInterestService,
                postcodeLookupService,
                providerDataService,
                sessionService);

        employerListModel.Input = new EmployerListModel.InputModel
        {
            SelectedPostcode = EmployerListModel.EnterPostcodeValue,
            CustomPostcode = ValidPostcode
        };
        
        await employerListModel.OnGet();

        employerListModel.EmployerInterestList
            .Should()
            .BeEquivalentTo(employerInterestSummary);

        employerListModel.ZeroResultsFound.Should().NotBeNull();
        employerListModel.ZeroResultsFound!.Value.Should().BeFalse();
        employerListModel.SelectedPostcodeHasFilters.Should().BeTrue();
    }

    [Fact]
    public async Task EmployerListModel_OnGet_Populates_EmployerInterest_List_For_Custom_Location()
    {
        var locationPostcode = new LocationPostcodeBuilder()
            .Build();

        var employerInterestSummary = new EmployerInterestSummaryBuilder()
            .BuildList()
            .ToList();

        var sessionService = Substitute.For<ISessionService>();
        sessionService
            .Get<LocationPostcode>(EmployerListModel.SessionKeyPostcodeLocation)
            .Returns(locationPostcode);

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .FindEmployerInterest(locationPostcode.Latitude, locationPostcode.Longitude)
            .Returns((employerInterestSummary, 2));

        var employerListModel = new EmployerListModelBuilder()
            .Build(employerInterestService,
                sessionService: sessionService);

        await employerListModel.OnGet();

        employerListModel.EmployerInterestList
            .Should()
            .BeEquivalentTo(employerInterestSummary);

        employerListModel.ZeroResultsFound.Should().NotBeNull();
        employerListModel.ZeroResultsFound!.Value.Should().BeFalse();
        employerListModel.SelectedPostcodeHasFilters.Should().BeFalse();
    }

    [Fact]
    public async Task EmployerListModel_OnGet_Sets_Expected_Values_For_Zero_Results_When_Session_Has_Postcode()
    {
        var locationPostcode = new LocationPostcodeBuilder()
            .Build();

        var sessionService = Substitute.For<ISessionService>();
        sessionService
            .Get<LocationPostcode>(EmployerListModel.SessionKeyPostcodeLocation)
            .Returns(locationPostcode);

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .FindEmployerInterest(locationPostcode.Latitude, locationPostcode.Longitude)
            .Returns((new List<EmployerInterestSummary>(), 0));

        var employerListModel = new EmployerListModelBuilder()
            .Build(employerInterestService,
                sessionService: sessionService);

        await employerListModel.OnGet();

        employerListModel.EmployerInterestList
            .Should()
            .BeEmpty();

        employerListModel.ZeroResultsFound.Should().NotBeNull();
        employerListModel.ZeroResultsFound!.Value.Should().BeTrue();
        employerListModel.SelectedPostcodeHasFilters.Should().BeFalse();
    }

    [Fact]
    public async Task EmployerListModel_OnGet_Sets_Expected_Provider_Locations_List()
    {
        var locationPostcodes = new LocationPostcodeBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetLocationPostcodes(PageContextBuilder.DefaultUkPrn)
            .Returns(locationPostcodes);

        var employerListModel = new EmployerListModelBuilder()
            .Build(providerDataService: providerDataService);

        await employerListModel.OnGet();

        employerListModel.ProviderLocations.Should().NotBeNull();
        employerListModel.ProviderLocations!
            .Count
            .Should()
            .Be(locationPostcodes.Count);
        employerListModel.ProviderLocations
            .Select(p => p.Value)
            .Should()
            .BeEquivalentTo(locationPostcodes);
    }

    [Fact]
    public async Task EmployerListModel_OnGet_Sets_Expected_Postcodes()
    {
        var locationPostcodes = new LocationPostcodeBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetLocationPostcodes(PageContextBuilder.DefaultUkPrn)
            .Returns(locationPostcodes);

        var employerListModel = new EmployerListModelBuilder()
            .Build(providerDataService: providerDataService);

        await employerListModel.OnGet();

        employerListModel.Postcodes.Should().NotBeNullOrEmpty();
        employerListModel.Postcodes!.Length.Should().Be(locationPostcodes.Count + 1);

        employerListModel.Postcodes
            .Last()
            .Should()
            .BeEquivalentTo(new SelectListItem(EmployerListModel.EnterPostcodeValue, EmployerListModel.EnterPostcodeValue));

        var orderedPostcodes = locationPostcodes.OrderBy(x => x.Name).ToArray();
        for (var i = 0; i < orderedPostcodes.Length; i++)
        {
            var campusName = orderedPostcodes[i].Name?.Length > 15
                ? orderedPostcodes[i].Name[..15] + "..."
                : orderedPostcodes[i].Name;
            var displayText = $"{campusName.ToUpper()} [{orderedPostcodes[i].Postcode}]";

            employerListModel.Postcodes[i].Text.Should().Be(displayText);
            employerListModel.Postcodes[i].Value.Should().Be(orderedPostcodes[i].Postcode);
        }

        employerListModel.Postcodes
            .Last()
            .Should()
            .BeEquivalentTo(new SelectListItem(EmployerListModel.EnterPostcodeValue, EmployerListModel.EnterPostcodeValue));
    }

    [Fact]
    public async Task EmployerListModel_OnGet_Sets_Expected_Postcodes_For_Provider_With_No_Locations()
    {
        var locationPostcodes = Enumerable.Empty<LocationPostcode>().ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetLocationPostcodes(PageContextBuilder.DefaultUkPrn)
            .Returns(locationPostcodes);

        var employerListModel = new EmployerListModelBuilder()
            .Build(providerDataService: providerDataService);

        await employerListModel.OnGet();

        employerListModel.Postcodes.Should().NotBeNullOrEmpty();
        employerListModel.Postcodes!.Length.Should().Be(1);

        employerListModel.Postcodes
            .Should()
            .Contain(x =>
                x.Text == EmployerListModel.EnterPostcodeValue &&
                x.Value == EmployerListModel.EnterPostcodeValue);
    }

    [Fact]
    public async Task EmployerListModel_OnPost_Redirects_To_Get()
    {
        var geoLocation = GeoLocationBuilder.BuildGeoLocation(ValidPostcode);
        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(ValidPostcode)
            .Returns(geoLocation);

        var employerListModel = new EmployerListModelBuilder()
            .Build(postcodeLookupService: postcodeLookupService);

        employerListModel.Input = new EmployerListModel.InputModel
        {
            SelectedPostcode = EmployerListModel.EnterPostcodeValue,
            CustomPostcode = ValidPostcode
        };

        var result = await employerListModel.OnPost();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Employer/EmployerList");
    }

    [Fact]
    public async Task EmployerListModel_OnPost_Sets_Expected_HasResults_To_False_For_Zero_Results()
    {
        var geoLocation = GeoLocationBuilder.BuildGeoLocation(ValidPostcode);
        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(ValidPostcode)
            .Returns(geoLocation);

        var employerListModel = new EmployerListModelBuilder()
            .Build(postcodeLookupService: postcodeLookupService);

        employerListModel.Input = new EmployerListModel.InputModel
        {
            SelectedPostcode = EmployerListModel.EnterPostcodeValue,
            CustomPostcode = ValidPostcode
        };

        var result = await employerListModel.OnPost();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Employer/EmployerList");
    }

    [Fact]
    public async Task EmployerListModel_OnPost_Validates_Empty_Custom_Postcode()
    {
        var geoLocation = GeoLocationBuilder.BuildGeoLocation(ValidPostcode);
        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(ValidPostcode)
            .Returns(geoLocation);

        var employerListModel = new EmployerListModelBuilder()
            .Build(postcodeLookupService: postcodeLookupService);

        employerListModel.Input = new EmployerListModel.InputModel
        {
            SelectedPostcode = EmployerListModel.EnterPostcodeValue,
            CustomPostcode = string.Empty
        };

        var result = await employerListModel.OnPost();

        var pageResult = result as PageResult;
        pageResult.Should().NotBeNull();

        employerListModel.ModelState.IsValid.Should().BeFalse();
        employerListModel.ModelState.Should().ContainKey(CustomPostcodeKey);
        employerListModel.ModelState[CustomPostcodeKey]!.Errors.Should().Contain(x => x.ErrorMessage == "Enter a postcode");
    }

    [Fact]
    public async Task EmployerListModel_OnPost_Validates_Invalid_Format_Custom_Postcode()
    {
        var geoLocation = GeoLocationBuilder.BuildGeoLocation(ValidPostcode);
        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(ValidPostcode)
            .Returns(geoLocation);

        var employerListModel = new EmployerListModelBuilder()
            .Build(postcodeLookupService: postcodeLookupService);

        employerListModel.Input = new EmployerListModel.InputModel
        {
            SelectedPostcode = EmployerListModel.EnterPostcodeValue,
            CustomPostcode = InvalidFormatPostcode
        };

        var result = await employerListModel.OnPost();

        var pageResult = result as PageResult;
        pageResult.Should().NotBeNull();

        employerListModel.ModelState.IsValid.Should().BeFalse();
        employerListModel.ModelState.Should().ContainKey(CustomPostcodeKey);
        employerListModel.ModelState[CustomPostcodeKey]!.Errors.Should().Contain(x => x.ErrorMessage == "Enter a postcode with numbers and letters only");
    }

    [Fact]
    public async Task EmployerListModel_OnPost_Validates_Invalid_Custom_Postcode()
    {
        var geoLocation = GeoLocationBuilder.BuildGeoLocation(ValidPostcode);
        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(ValidPostcode)
            .Returns(geoLocation);

        var employerListModel = new EmployerListModelBuilder()
            .Build(postcodeLookupService: postcodeLookupService);

        employerListModel.Input = new EmployerListModel.InputModel
        {
            SelectedPostcode = EmployerListModel.EnterPostcodeValue,
            CustomPostcode = InvalidPostcode
        };

        var result = await employerListModel.OnPost();

        var pageResult = result as PageResult;
        pageResult.Should().NotBeNull();

        employerListModel.ModelState.IsValid.Should().BeFalse();
        employerListModel.ModelState.Should().ContainKey(CustomPostcodeKey);
        employerListModel.ModelState[CustomPostcodeKey]!.Errors.Should().Contain(x => x.ErrorMessage == "Enter a real postcode");
    }
}
