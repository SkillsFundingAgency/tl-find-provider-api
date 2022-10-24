using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Application.Models.Exceptions;

namespace Sfa.Tl.Find.Provider.Application.Services;
public class EmployerInterestService : IEmployerInterestService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IEmailService _emailService;
    private readonly IPostcodeLookupService _postcodeLookupService;
    private readonly IEmployerInterestRepository _employerInterestRepository;
    private readonly IProviderDataService _providerDataService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<EmployerInterestService> _logger;
    private readonly EmployerInterestSettings _employerInterestSettings;

    public EmployerInterestService(
        IDateTimeService dateTimeService,
        IEmailService emailService,
        IPostcodeLookupService postcodeLookupService,
        IProviderDataService providerDataService,
        IEmployerInterestRepository employerInterestRepository,
        IMemoryCache cache,
        IOptions<EmployerInterestSettings> employerInterestOptions,
        ILogger<EmployerInterestService> logger)
    {
        _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _postcodeLookupService = postcodeLookupService ?? throw new ArgumentNullException(nameof(postcodeLookupService));
        _employerInterestRepository = employerInterestRepository ?? throw new ArgumentNullException(nameof(employerInterestRepository));
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _employerInterestSettings = employerInterestOptions?.Value
                                    ?? throw new ArgumentNullException(nameof(employerInterestOptions));
    }

    public int RetentionDays => _employerInterestSettings.RetentionDays;


    public async Task<Guid> CreateEmployerInterest(EmployerInterest employerInterest)
    {
        var geoLocation = await GetPostcode(employerInterest.Postcode);

        //Create a copy - will find a cleaner way to do this later
        var cleanEmployerInterest = new EmployerInterest
        {
            OrganisationName = employerInterest.OrganisationName?.Trim(),
            ContactName = employerInterest.ContactName?.Trim(),
            Postcode = geoLocation.Location,
            Latitude = geoLocation.Latitude,
            Longitude = geoLocation.Longitude,
            IndustryId = employerInterest.IndustryId,
            OtherIndustry = employerInterest.OtherIndustry?.ToTrimmedOrNullString(),
            AdditionalInformation = employerInterest.AdditionalInformation?.ToTrimmedOrNullString(),
            Email = employerInterest.Email?.ToTrimmedOrNullString(),
            Telephone = employerInterest.Telephone?.ToTrimmedOrNullString(),
            Website = employerInterest.Website?.ToTrimmedOrNullString(),
            ContactPreferenceType = employerInterest.ContactPreferenceType,
            SkillAreaIds = employerInterest.SkillAreaIds
        };

        var (_, uniqueId) = await _employerInterestRepository.Create(cleanEmployerInterest);

        if (uniqueId != Guid.Empty)
        {
            cleanEmployerInterest.UniqueId = uniqueId;
            await SendEmployerRegisterInterestEmail(cleanEmployerInterest);
        }

        return uniqueId;
    }

    public async Task<int> DeleteEmployerInterest(Guid uniqueId)
    {
        var count = await _employerInterestRepository.Delete(uniqueId);

        _logger.LogInformation("Removed {count} employer interest records with unique id {uniqueId}",
            count, uniqueId);

        return count;
    }

    public async Task<int> RemoveExpiredEmployerInterest()
    {
        if (_employerInterestSettings.RetentionDays <= 0)
        {
            _logger.LogInformation("{service} {method} processing skipped because retention dates was not greater than zero.",
                nameof(EmployerInterestService), nameof(RemoveExpiredEmployerInterest));
            return 0;
        }

        var date = _dateTimeService.Today.AddDays(-_employerInterestSettings.RetentionDays);
        var count = await _employerInterestRepository.DeleteBefore(date);

        _logger.LogInformation("Removed {count} employer interest records because they are over {days} days (older than {date:yyyy-MM-dd})",
            count, _employerInterestSettings.RetentionDays, date);

        return count;
    }

    public Task<IEnumerable<EmployerInterestSummary>> FindEmployerInterest()
    {
        //TODO: Create a find method that takes lat/long or location
        return _employerInterestRepository.GetSummaryList();
    }

    public Task<EmployerInterestDetail> GetEmployerInterestDetail(int id)
    {
        return _employerInterestRepository.GetDetail(id);
    }

    private async Task<bool> SendEmployerRegisterInterestEmail(EmployerInterest employerInterest)
    {
        var unsubscribeUri = new Uri(QueryHelpers.AddQueryString(
            _employerInterestSettings.UnsubscribeEmployerUri.TrimEnd('/'),
            "id",
            employerInterest.UniqueId.ToString("D").ToLower()));

        var contactPreference = employerInterest.ContactPreferenceType switch
        {
            1 => "Email",
            2 => "Telephone",
            _ => "No preference"
        };

        var industries = await _providerDataService.GetIndustries();
        var routes = await _providerDataService.GetRoutes();

        var industry = industries
            .FirstOrDefault(i => i.Id == employerInterest.IndustryId)
            ?.Name ?? employerInterest.OtherIndustry;

        var skillAreas = employerInterest.SkillAreaIds?.ToList() ?? new List<int>();
        var placementAreas = string.Join(", ", routes
            .Where(r => skillAreas.Contains(r.Id))
            .OrderBy(r => r.Name)
            .Select(r => r.Name)
        );

        var tokens = new Dictionary<string, string>
        {
            { "contact_name", employerInterest.ContactName },
            { "email_address", employerInterest.Email ?? "" },
            { "telephone", employerInterest.Telephone ?? "" },
            { "contact_preference", contactPreference },
            { "organisation_name", employerInterest.OrganisationName },
            { "website", employerInterest.Website ?? "" },
            { "primary_industry", industry },
            { "placement_area", placementAreas },
            { "has_multiple_placement_areas", skillAreas.Count > 1 ? "yes" : "no" },
            { "postcode", employerInterest.Postcode },
            { "additional_information", employerInterest.AdditionalInformation },
            { "employer_support_site", _employerInterestSettings.EmployerSupportSiteUri },
            { "employer_unsubscribe_uri", unsubscribeUri.ToString() }
        };

        return await _emailService.SendEmail(
            employerInterest.Email,
            EmailTemplateNames.EmployerRegisterInterest,
            tokens);
    }

    private async Task<GeoLocation> GetPostcode(string postcode)
    {
        var key = CacheKeys.PostcodeKey(postcode);

        if (!_cache.TryGetValue(key, out GeoLocation geoLocation))
        {
            geoLocation = postcode.Length <= 4
                ? await _postcodeLookupService.GetOutcode(postcode)
                : await _postcodeLookupService.GetPostcode(postcode);

            if (geoLocation is null)
            {
                throw new PostcodeNotFoundException(postcode);
            }

            _cache.Set(key, geoLocation,
                CacheUtilities.DefaultMemoryCacheEntryOptions(
                    _dateTimeService,
                    _logger));
        }

        return geoLocation;
    }
}
