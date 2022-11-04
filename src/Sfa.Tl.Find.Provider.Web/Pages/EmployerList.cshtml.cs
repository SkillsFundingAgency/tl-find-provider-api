using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Web.Pages;

//TODO: add security
[AllowAnonymous]
//[Authorize(nameof(PolicyNames.EmployerInterestViewer))]
public class EmployerListModel : PageModel
{
    private readonly IEmployerInterestService _employerInterestService;
    private readonly IPostcodeLookupService _postcodeLookupService;
    private readonly IProviderDataService _providerDataService;
    private readonly ISessionService _sessionService;
    private readonly EmployerInterestSettings _employerInterestSettings;
    private readonly ILogger<EmployerListModel> _logger;

    public const string SessionKeyPostcodeLocation = "_EmployerList_PostcodeLocation";
    public const string EnterPostcodeValue = "Enter postcode";

    public IEnumerable<EmployerInterestSummary>? EmployerInterestList { get; private set; }

    public IEnumerable<LocationPostcode>? ProviderLocations { get; private set; }

    public SelectListItem[]? Postcodes { get; private set; }

    public int EmployerInterestRetentionDays =>
        _employerInterestSettings.RetentionDays;

    public int EmployerInterestRetentionWeeks =>
        (int)Math.Ceiling(_employerInterestSettings.RetentionDays / 7d);

    public int SearchRadius =>
        _employerInterestSettings.SearchRadius;

    public DateOnly? ServiceStartDate =>
        _employerInterestSettings?.ServiceStartDate is not null
            ? DateOnly.FromDateTime(_employerInterestSettings.ServiceStartDate.Value)
            : DateOnly.MinValue;

    public long? UkPrn { get; private set; }

    public bool? ZeroResultsFound { get; set; }

    [BindProperty] public InputModel? Input { get; set; }

    public EmployerListModel(
        IEmployerInterestService employerInterestService,
        IPostcodeLookupService postcodeLookupService,
        IProviderDataService providerDataService,
        ISessionService sessionService,
        IOptions<EmployerInterestSettings> employerInterestOptions,
        ILogger<EmployerListModel> logger)
    {
        _employerInterestService =
            employerInterestService ?? throw new ArgumentNullException(nameof(employerInterestService));
        _postcodeLookupService = postcodeLookupService ?? throw new ArgumentNullException(nameof(postcodeLookupService));
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _employerInterestSettings = employerInterestOptions?.Value
                                    ?? throw new ArgumentNullException(nameof(employerInterestOptions));
    }

    public async Task OnGet()
    {
        UkPrn = GetUkPrn();
        await LoadProviderPostcodes(UkPrn);

        var postcodeLocation = _sessionService.Get<LocationPostcode>(SessionKeyPostcodeLocation);

        if (postcodeLocation is not null)
        {
            Input ??= new InputModel();
            if (ProviderLocations != null && ProviderLocations.Any(p => p.Postcode == postcodeLocation.Postcode))
            {
                Input.SelectedPostcode = postcodeLocation.Postcode;
            }
            else
            {
                Input.SelectedPostcode = EnterPostcodeValue;
                Input.CustomPostcode = postcodeLocation.Postcode;
            }

            await PerformSearch(postcodeLocation);
        }
    }

    public async Task<IActionResult> OnPost()
    {
        UkPrn = GetUkPrn();
        await LoadProviderPostcodes(UkPrn);
        ZeroResultsFound = false;

        //Validation - must have either a valid selected postcode, or a non-empty custom postcode
        if (Input?.SelectedPostcode == EnterPostcodeValue && string.IsNullOrEmpty(Input.CustomPostcode))
        {
            ModelState.AddModelError($"{nameof(Input)}.{nameof(Input.CustomPostcode)}", "Enter a postcode");
        }
        
        LocationPostcode postcodeLocation = null;
        //TODO: Extract to method and add tests
        if (Input?.SelectedPostcode != EnterPostcodeValue && !string.IsNullOrEmpty(Input?.SelectedPostcode))
        {
            //TODO: Use a dictionary here?
            postcodeLocation = ProviderLocations?.FirstOrDefault(p => p.Postcode == Input.SelectedPostcode);
        }
        else
        {
            var geoLocation = Input.CustomPostcode.Length <= 4
                ? await _postcodeLookupService.GetOutcode(Input.CustomPostcode)
                : await _postcodeLookupService.GetPostcode(Input.CustomPostcode);

            if (geoLocation is not null)
            {
                postcodeLocation = new LocationPostcode
                {
                    Postcode = geoLocation.Location,
                    Latitude = geoLocation.Latitude,
                    Longitude = geoLocation.Longitude
                };
            }
        }

        if (postcodeLocation is null)
        {
            ModelState.AddModelError($"{nameof(Input)}.{nameof(Input.CustomPostcode)}", "Enter a real postcode");
        }
        
        if (!ModelState.IsValid)
        {
            if (_sessionService.Exists(SessionKeyPostcodeLocation))
            {
                _sessionService.Remove(SessionKeyPostcodeLocation);
            }
            return Page();
        }

        _sessionService.Set(SessionKeyPostcodeLocation, postcodeLocation);

        return RedirectToPage("/EmployerList");

        //await PerformSearch(postcodeLocation);
    }

    private long? GetUkPrn()
    {
        var ukPrnClaim = HttpContext.User.GetClaim(CustomClaimTypes.UkPrn);
        return ukPrnClaim is not null && long.TryParse(ukPrnClaim, out var ukPrn)
            ? ukPrn
            : default;
    }

    private async Task LoadProviderPostcodes(long? ukPrn)
    {
        if (ukPrn is null) return;

        ProviderLocations = await _providerDataService
            .GetLocationPostcodes(ukPrn.Value);

        Postcodes = ProviderLocations.Select(p
                    => new SelectListItem(
                        p.Postcode,
                        p.Postcode,
                        p.Postcode ==
                        Input?.SelectedPostcode) //Selected = false //TODO: set based on previous selection
            )
            //Select postcode
            //.Prepend(new SelectListItem("Select postcode", "", true))
            .Append(new SelectListItem(EnterPostcodeValue, EnterPostcodeValue, Input?.SelectedPostcode == EnterPostcodeValue))
            .ToArray();
    }

    private async Task PerformSearch()
    {
        var locationLookup = !string.IsNullOrEmpty(Input?.SelectedPostcode)
                ? ProviderLocations?.FirstOrDefault(p => p.Postcode == Input.SelectedPostcode)
                : null;

        if (locationLookup != null)
        {
            EmployerInterestList = await _employerInterestService.FindEmployerInterest(locationLookup.Latitude, locationLookup.Longitude);
            ZeroResultsFound = !EmployerInterestList.Any();
        }
        else if (!string.IsNullOrEmpty(Input?.CustomPostcode))
        {
            (EmployerInterestList, _) = await _employerInterestService.FindEmployerInterest(Input?.CustomPostcode);
            ZeroResultsFound = !EmployerInterestList.Any();
        }
    }

    private async Task PerformSearch(LocationPostcode postcodeLocation)
    {
        EmployerInterestList = await _employerInterestService.FindEmployerInterest(postcodeLocation.Latitude, postcodeLocation.Longitude);
        ZeroResultsFound = !EmployerInterestList.Any();
    }

    public class InputModel
    {
        public string? SelectedPostcode { get; set; }

        public string? CustomPostcode { get; set; }
    }
}