using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages.Employer;

[Authorize(nameof(PolicyNames.IsProviderOrAdministrator))]
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

    public IDictionary<string, LocationPostcode>? ProviderLocations { get; private set; }

    public SelectListItem[]? Postcodes { get; private set; }

    [TempData]
    public string? DeletedOrganisationName { get; set; }

    public int EmployerInterestRetentionDays =>
        _employerInterestSettings.RetentionDays;

    public int EmployerInterestRetentionWeeks =>
        (int)Math.Ceiling(_employerInterestSettings.RetentionDays / 7d);

    public int SearchRadius =>
        _employerInterestSettings.SearchRadius;

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
        if (UkPrn is not null && UkPrn > 0)
        {
            await LoadProviderView(UkPrn.Value);
        }
        else if(User.IsInRole(CustomRoles.Administrator))
        {
            await LoadAdministratorView();
        }
    }

    private async Task LoadAdministratorView()
    {
        EmployerInterestList = await _employerInterestService
                .GetSummaryList();
        ZeroResultsFound = !EmployerInterestList.Any();
    }

    private async Task LoadProviderView(long ukPrn)
    {
        await LoadProviderPostcodes(ukPrn);

        var postcodeLocation = _sessionService.Get<LocationPostcode>(SessionKeyPostcodeLocation);

        if (postcodeLocation is not null)
        {
            Input ??= new InputModel();
            if (ProviderLocations != null && ProviderLocations.ContainsKey(postcodeLocation.Postcode))
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

        LocationPostcode? postcodeLocation = null;
        
        //User has selected a postcode from the dropdown
        if (Input?.SelectedPostcode != EnterPostcodeValue)
        {
            if (!string.IsNullOrEmpty(Input?.SelectedPostcode))
            {
                postcodeLocation = ProviderLocations?.TryGetValue(Input?.SelectedPostcode!, out var loc) == true
                    ? loc : null;
            }

            if (postcodeLocation is null)
            {
                ModelState.AddModelError($"{nameof(Input)}.{nameof(Input.CustomPostcode)}", "Enter a real postcode");
            }
        }

        //User has selected to enter a custom postcode
        if (Input?.SelectedPostcode == EnterPostcodeValue)
        {
            if (string.IsNullOrEmpty(Input.CustomPostcode))
            {
                ModelState.AddModelError($"{nameof(Input)}.{nameof(Input.CustomPostcode)}", "Enter a postcode");
            }
            else if (!Input.CustomPostcode.IsFullOrPartialPostcode())
            {
                ModelState.AddModelError($"{nameof(Input)}.{nameof(Input.CustomPostcode)}",
                    "Enter a postcode with numbers and letters only");
            }

            if (ModelState.IsValid)
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
                else
                {
                    ModelState.AddModelError($"{nameof(Input)}.{nameof(Input.CustomPostcode)}",
                        "Enter a real postcode");
                }
            }
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

        return RedirectToPage("/Employer/EmployerList");
    }

    private long? GetUkPrn()
    {
        var ukPrnClaim = HttpContext.User.GetClaim(CustomClaimTypes.UkPrn);
        return ukPrnClaim is not null && long.TryParse(ukPrnClaim, out var ukPrn)
            ? ukPrn
            : null;
    }

    private async Task LoadProviderPostcodes(long? ukPrn)
    {
        if (ukPrn is null) return;

        ProviderLocations = (await _providerDataService
            .GetLocationPostcodes(ukPrn.Value))
            ?.ToDictionary(
                x => x.Postcode,
                x => x);

        Postcodes = ProviderLocations?.Select(p
                    => new SelectListItem(
                        $"{p.Value.Name} [{p.Value.Postcode}]",
                        p.Key,
                        p.Key == Input?.SelectedPostcode)
            )
            .OrderBy(x => x.Text)
            //.Prepend(new SelectListItem("Select postcode", "", true))
            .Append(new SelectListItem(EnterPostcodeValue, EnterPostcodeValue, Input?.SelectedPostcode == EnterPostcodeValue))
            .ToArray();
    }

    private async Task PerformSearch(LocationPostcode postcodeLocation)
    {
        EmployerInterestList = (await _employerInterestService
            .FindEmployerInterest(postcodeLocation.Latitude, postcodeLocation.Longitude))
            .SearchResults;
        ZeroResultsFound = !EmployerInterestList.Any();
    }

    public class InputModel
    {
        public string? SelectedPostcode { get; set; }

        public string? CustomPostcode { get; set; }
    }
}