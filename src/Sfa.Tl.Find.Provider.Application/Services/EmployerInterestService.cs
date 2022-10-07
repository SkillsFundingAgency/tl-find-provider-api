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
    private readonly IMemoryCache _cache;
    private readonly ILogger<EmployerInterestService> _logger;
    private readonly EmployerInterestSettings _employerInterestSettings;

    public EmployerInterestService(
        IDateTimeService dateTimeService,
        IEmailService emailService,
        IPostcodeLookupService postcodeLookupService,
        IEmployerInterestRepository employerInterestRepository,
        IMemoryCache cache,
        IOptions<EmployerInterestSettings> employerInterestOptions,
        ILogger<EmployerInterestService> logger)
    {
        _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _postcodeLookupService = postcodeLookupService ?? throw new ArgumentNullException(nameof(postcodeLookupService));
        _employerInterestRepository = employerInterestRepository ?? throw new ArgumentNullException(nameof(employerInterestRepository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _employerInterestSettings = employerInterestOptions?.Value
                                    ?? throw new ArgumentNullException(nameof(employerInterestOptions));
    }

    public async Task<Guid> CreateEmployerInterest(EmployerInterest employerInterest)
    {
        var geoLocation = await GetPostcode(employerInterest.Postcode);

        //Create a copy - will find a cleaner way to do this later
        employerInterest = new EmployerInterest
        {
            OrganisationName = employerInterest.OrganisationName,
            ContactName = employerInterest.ContactName,
            Postcode = geoLocation.Location,
            Latitude = geoLocation.Latitude,
            Longitude = geoLocation.Longitude,
            HasMultipleLocations = employerInterest.HasMultipleLocations,
            LocationCount = employerInterest.LocationCount,
            IndustryId = employerInterest.IndustryId,
            SpecificRequirements = employerInterest.SpecificRequirements,
            Email = employerInterest.Email,
            Telephone = employerInterest.Telephone,
            ContactPreferenceType = employerInterest.ContactPreferenceType
        };

        var (count, uniqueId) = await _employerInterestRepository.Create(employerInterest);

        if (uniqueId != Guid.Empty)
        {
            employerInterest.UniqueId = uniqueId;
            await SendEmployerRegisterInterestEmail(employerInterest);
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

    public Task<IEnumerable<EmployerInterest>> FindEmployerInterest()
    {
        return _employerInterestRepository.GetAll();
    }

    public async Task<bool> SendEmployerRegisterInterestEmail(EmployerInterest employerInterest)
    {
        //TODO: Move to constants class:
        //EmailTemplateNames.EmployerRegisterInterest
        const string templateName = "EmployerRegisterInterest";

        var unsubscribeUri = new Uri(QueryHelpers.AddQueryString(
            _employerInterestSettings.UnsubscribeEmployerUri.TrimEnd('/'),
            "id",
            employerInterest.UniqueId.ToString("D").ToLower()));
        
        //https://localhost:7191/EmployerInterest/Unsubscribe?id=A78BA9E5-EB8B-4FF8-ABFC-36035A029AC1
        //    "UnsubscribeEmployerUri": "https://localhost:7191/EmployerInterest/Unsubscribe",

        var contactPreference = employerInterest.ContactPreferenceType switch
        {
            1 => "Email",
            2 => "Telephone",
            _ => "Unknown"
        };

        var howManyLocations =
        employerInterest.HasMultipleLocations && employerInterest.LocationCount > 0
            ? $"{employerInterest.LocationCount} location {(employerInterest.LocationCount > 1 ? "s" : "")}"
                : "A single location";

        var tokens = new Dictionary<string, string>
        {
            { "organisation_name", employerInterest.OrganisationName },
            { "contact_name", employerInterest.ContactName },
            { "email_address", employerInterest.Email },
            { "telephone", employerInterest.Telephone },
            { "contact_preference", contactPreference },
            //Need to convert to text
            { "primary_industry", employerInterest.IndustryId.ToString() },
            { "postcode", employerInterest.Postcode },
            { "specific_requirements", employerInterest.SpecificRequirements },
            { "how_many_locations", howManyLocations },
            { "employer_support_site", _employerInterestSettings.EmployerSupportSiteUri },
            { "unique_id", employerInterest.UniqueId.ToString() },
            { "employer_unsubscribe_uri", unsubscribeUri.ToString() }
        };

        return await _emailService.SendEmail(
            employerInterest.Email,
            templateName,
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
