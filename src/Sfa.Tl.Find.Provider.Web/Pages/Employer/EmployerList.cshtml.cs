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
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class EmployerListModel : PageModel
{
    private readonly IEmployerInterestService _employerInterestService;
    private readonly IPostcodeLookupService _postcodeLookupService;
    private readonly IProviderDataService _providerDataService;
    private readonly ISessionService _sessionService;
    private readonly EmployerInterestSettings _employerInterestSettings;
    private readonly ILogger<EmployerListModel> _logger;

    public const string SessionKeyPostcodeLocation = "_EmployerList_PostcodeLocation";
    public const string SessionKeySelectedSortColumn = "_EmployerList_SelectedSortColumn";
    public const string EnterPostcodeValue = "Enter postcode";

    public const string SortColumnOrganisationName = "OrganisationName";
    public const string SortColumnDistance = "Distance";
    public const string SortColumnExpiryDate = "ExpiryDate";

    public Dictionary<string, string> SortColumnMappings { get; } = new()
    {
        { SortColumnOrganisationName, "Organisation" },
        { SortColumnDistance, "Distance" },
        { SortColumnExpiryDate, "Expiry Date" }
    };

    public IEnumerable<EmployerInterestSummary>? EmployerInterestList { get; private set; }

    public IDictionary<string, LocationPostcode>? ProviderLocations { get; private set; }

    public SelectListItem[]? Postcodes { get; private set; }

    public SelectListItem[]? SortColumns { get; private set; }

    public int? SelectedLocationId { get; private set; }

    public bool SelectedPostcodeHasFilters { get; private set; }

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
        UkPrn = HttpContext.User.GetUkPrn();

        MapSortColumns();

        if (UkPrn is not null && UkPrn > 0)
        {
            await LoadProviderView(UkPrn.Value);
        }
        else if (User.IsInRole(CustomRoles.Administrator))
        {
            await LoadAdministratorView();
        }
    }

    private void MapSortColumns()
    {
        SortColumns = Array.Empty<SelectListItem>();

        foreach (var item in SortColumnMappings)
        {
            SortColumns = SortColumns.Append(new SelectListItem(item.Value, item.Key)).ToArray();
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
        var selectedSortColumn = _sessionService.Get<string?>(SessionKeySelectedSortColumn);

        if (postcodeLocation is not null)
        {
            Input ??= new InputModel();
            if (ProviderLocations != null &&
                ProviderLocations.ContainsKey(postcodeLocation.Postcode) &&
                postcodeLocation.Id is not null)
            {
                Input.SelectedPostcode = postcodeLocation.Postcode;
                SelectedLocationId = postcodeLocation.Id;
                Input.SelectedSortColumn = selectedSortColumn;
                await PerformSearch(postcodeLocation.Id.Value);
            }
            else
            {
                Input.SelectedPostcode = EnterPostcodeValue;
                Input.CustomPostcode = postcodeLocation.Postcode;
                Input.SelectedSortColumn = selectedSortColumn;
                await PerformSearch(postcodeLocation);
            }
        }
    }

    public async Task<IActionResult> OnPost()
    {
        UkPrn = HttpContext.User.GetUkPrn();
        await LoadProviderPostcodes(UkPrn);
        ZeroResultsFound = false;

        var selectedSortColumn = Input?.SelectedSortColumn;

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
        _sessionService.Set(SessionKeySelectedSortColumn, selectedSortColumn);

        return RedirectToPage("/Employer/EmployerList");
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
                        $"{p.Value.Name.TruncateWithEllipsis(15).ToUpper()} [{p.Value.Postcode}]",
                        p.Key,
                        p.Key == Input?.SelectedPostcode)
            )
            .OrderBy(x => x.Text)
            .Append(new SelectListItem(EnterPostcodeValue, EnterPostcodeValue, Input?.SelectedPostcode == EnterPostcodeValue))
            .ToArray();
    }

    private static IEnumerable<EmployerInterestSummary> SortEmployerInterestList(
        IEnumerable<EmployerInterestSummary> employerInterestList,
        string? selectedSortColumn)
    {
        return selectedSortColumn switch
        {
            SortColumnOrganisationName => employerInterestList.OrderBy(x => x.OrganisationName),
            SortColumnDistance => employerInterestList.OrderBy(x => x.Distance),
            SortColumnExpiryDate => employerInterestList.OrderBy(x => x.ExpiryDate),
            _ => employerInterestList
        };
    }

    private async Task PerformSearch(int locationId)
    {
        (EmployerInterestList, _, SelectedPostcodeHasFilters) =
            await _employerInterestService.FindEmployerInterest(locationId);

        EmployerInterestList = SortEmployerInterestList(EmployerInterestList, Input?.SelectedSortColumn).ToList();

        ZeroResultsFound = !EmployerInterestList.Any();
    }

    private async Task PerformSearch(LocationPostcode postcodeLocation)
    {
        (EmployerInterestList, _) =
            await _employerInterestService
            .FindEmployerInterest(postcodeLocation.Latitude, postcodeLocation.Longitude);

        EmployerInterestList = SortEmployerInterestList(EmployerInterestList, Input?.SelectedSortColumn).ToList();

        ZeroResultsFound = !EmployerInterestList.Any();
        SelectedPostcodeHasFilters = false;
    }

    public class InputModel
    {
        public string? SelectedPostcode { get; set; }

        public string? CustomPostcode { get; set; }

        public string? SelectedSortColumn { get; set; }
    }
}