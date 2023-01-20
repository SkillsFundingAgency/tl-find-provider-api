using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Sfa.Tl.Find.Provider.Application.Models.Exceptions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.Services;
public class EmployerInterestService : IEmployerInterestService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IEmailService _emailService;
    private readonly IPostcodeLookupService _postcodeLookupService;
    private readonly IEmployerInterestRepository _employerInterestRepository;
    private readonly IProviderDataService _providerDataService;
    private readonly ILogger<EmployerInterestService> _logger;
    private readonly EmployerInterestSettings _employerInterestSettings;

    public EmployerInterestService(
        IDateTimeProvider dateTimeProvider,
        IEmailService emailService,
        IPostcodeLookupService postcodeLookupService,
        IProviderDataService providerDataService,
        IEmployerInterestRepository employerInterestRepository,
        IOptions<EmployerInterestSettings> employerInterestOptions,
        ILogger<EmployerInterestService> logger)
    {
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _postcodeLookupService = postcodeLookupService ?? throw new ArgumentNullException(nameof(postcodeLookupService));
        _employerInterestRepository = employerInterestRepository ?? throw new ArgumentNullException(nameof(employerInterestRepository));
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _employerInterestSettings = employerInterestOptions?.Value
                                    ?? throw new ArgumentNullException(nameof(employerInterestOptions));
    }

    public int RetentionDays => _employerInterestSettings.RetentionDays;

    public async Task<Guid> CreateEmployerInterest(EmployerInterest employerInterest)
    {
        var geoLocation = await GetPostcode(employerInterest.Postcode);

        var expiryDate = _dateTimeProvider
            .Today
            .AddDays(_employerInterestSettings.RetentionDays + 1)
            .AddTicks(-1); //Set to last instant of the expiry day

        var (_, uniqueId) = await _employerInterestRepository
            .Create(
                employerInterest,
                geoLocation,
                expiryDate);

        if (uniqueId != Guid.Empty)
        {
            employerInterest.UniqueId = uniqueId;
            await SendEmployerRegisterInterestEmail(employerInterest, geoLocation);
        }

        return uniqueId;
    }

    public async Task<int> DeleteEmployerInterest(int id)
    {
        var count = await _employerInterestRepository.Delete(id);

        _logger.LogInformation("Removed {count} employer interest records with id {id}",
            count, id);

        return count;
    }

    public async Task<int> DeleteEmployerInterest(Guid uniqueId)
    {
        var count = await _employerInterestRepository.Delete(uniqueId);

        _logger.LogInformation("Removed {count} employer interest records with unique id {uniqueId}",
            count, uniqueId);

        return count;
    }

    public async Task<bool> ExtendEmployerInterest(Guid id)
    {
        return await _employerInterestRepository
            .ExtendExpiry(id,
                _employerInterestSettings.RetentionDays,
                _employerInterestSettings.ExpiryNotificationDays);
    }

    public async Task<int> NotifyExpiringEmployerInterest()
    {
        if (_employerInterestSettings.RetentionDays <= 0)
        {
            _logger.LogInformation("{service} {method} processing skipped because retention dates was not greater than zero.",
                nameof(EmployerInterestService), nameof(NotifyExpiringEmployerInterest));
            return 0;
        }

        var expiringInterestList = (await _employerInterestRepository
            .GetExpiringInterest(_employerInterestSettings.ExpiryNotificationDays))
            .ToList();

        foreach (var employerInterest in expiringInterestList)
        {
            await SendEmployerExtendInterestEmail(employerInterest);
            await _employerInterestRepository.UpdateExtensionEmailSentDate(employerInterest.Id);
        }

        _logger.LogInformation("Notified {count} employers that their interest is about to expire",
            expiringInterestList.Count);

        return expiringInterestList.Count;
    }

    public async Task<int> RemoveExpiredEmployerInterest()
    {
        if (_employerInterestSettings.RetentionDays <= 0)
        {
            _logger.LogInformation("{service} {method} processing skipped because retention dates was not greater than zero.",
                nameof(EmployerInterestService), nameof(RemoveExpiredEmployerInterest));
            return 0;
        }

        var expiredInterest = (await _employerInterestRepository
                .DeleteExpired(_dateTimeProvider.Today))
            .ToList();

        if (expiredInterest.Any())
        {
            _logger.LogInformation("Removed {count} expired employer interest records",
                expiredInterest.Count);

            foreach (var item in expiredInterest)
            {
                await SendEmployerInterestRemovedEmail(item);
            }
        }

        return expiredInterest.Count;
    }

    public async Task<IEnumerable<EmployerInterestSummary>> GetSummaryList()
    {
        return (await _employerInterestRepository
            .GetSummaryList())
            .SetSummaryListFlags(_dateTimeProvider.Today);
    }

    public async Task<(IEnumerable<EmployerInterestSummary> SearchResults, int TotalResultsCount, bool SearchFiltersApplied)>
        FindEmployerInterest(int locationId)
    {
        var (searchResults, totalResultsCount, searchFiltersApplied) = 
            await _employerInterestRepository
                .Search(locationId, _employerInterestSettings.SearchRadius);

        var summaryList = searchResults
            .SetSummaryListFlags(_dateTimeProvider.Today);

        return (summaryList, TotalResultsCount: totalResultsCount, SearchFiltersApplied: searchFiltersApplied);
    }

    public async Task<(IEnumerable<EmployerInterestSummary> SearchResults, int TotalResultsCount)>
        FindEmployerInterest(string postcode)
    {
        var geoLocation = await GetPostcode(postcode);

        return await FindEmployerInterest(
            geoLocation.Latitude,
            geoLocation.Longitude);
    }

    public async Task<(IEnumerable<EmployerInterestSummary> SearchResults, int TotalResultsCount)>
        FindEmployerInterest(
            double latitude,
            double longitude)
    {
        var (searchResults, totalResultsCount) = 
            await _employerInterestRepository
                .Search(latitude, longitude, _employerInterestSettings.SearchRadius);

        var summaryList = searchResults
            .SetSummaryListFlags(_dateTimeProvider.Today);

        return (summaryList, TotalResultsCount: totalResultsCount);
    }

    public Task<EmployerInterestDetail> GetEmployerInterestDetail(int id)
    {
        return _employerInterestRepository.GetDetail(id);
    }

    private async Task<bool> SendEmployerExtendInterestEmail(EmployerInterest employerInterest)
    {
        var geoLocation = new GeoLocation
        {
            Location = employerInterest.Postcode,
            Latitude = employerInterest.Latitude,
            Longitude = employerInterest.Longitude
        };

        return await _emailService.SendEmail(
            employerInterest.Email,
            EmailTemplateNames.EmployerExtendInterest,
            new Dictionary<string, string>
            {
                { "details_list", await BuildEmployerInterestDetailsList(employerInterest, geoLocation) },
                { "employer_extend_uri", BuildUriWithUniqueId(_employerInterestSettings.ExtendEmployerUri, employerInterest) },
                { "employer_support_site", _employerInterestSettings.EmployerSupportSiteUri ?? "" },
                { "employer_unsubscribe_uri", BuildUriWithUniqueId(_employerInterestSettings.UnsubscribeEmployerUri, employerInterest) }
            },
            employerInterest.UniqueId.ToString());
    }

    private async Task<bool> SendEmployerInterestRemovedEmail(ExpiredEmployerInterestDto expiredInterest)
    {
        return await _emailService.SendEmail(
            expiredInterest.Email,
            EmailTemplateNames.EmployerInterestRemoved,
            new Dictionary<string, string>
            {
                { "organisation_name", expiredInterest.OrganisationName ?? "" },
                { "postcode", expiredInterest.Postcode ?? "" },
                { "register_interest_uri", _employerInterestSettings.RegisterInterestUri ?? "" },
                { "employer_support_site", _employerInterestSettings.EmployerSupportSiteUri ?? "" }
            },
            expiredInterest.UniqueId.ToString());
    }

    private async Task<bool> SendEmployerRegisterInterestEmail(EmployerInterest employerInterest, GeoLocation geoLocation)
    {
        return await _emailService.SendEmail(
            employerInterest.Email,
            EmailTemplateNames.EmployerRegisterInterest,
            new Dictionary<string, string>
            {
                { "details_list", await BuildEmployerInterestDetailsList(employerInterest, geoLocation) },
                { "employer_support_site", _employerInterestSettings.EmployerSupportSiteUri ?? "" },
                { "employer_unsubscribe_uri", BuildUriWithUniqueId(_employerInterestSettings.UnsubscribeEmployerUri, employerInterest) }
            },
            employerInterest.UniqueId.ToString());
    }

    private static string BuildUriWithUniqueId(string baseUri, EmployerInterest employerInterest)
    {
        return !string.IsNullOrEmpty(baseUri)
            ? new Uri(QueryHelpers.AddQueryString(
                baseUri.TrimEnd('/'),
                "id",
                employerInterest.UniqueId.ToString("D").ToLower()))
                .ToString()
            : "";
    }

    private async Task<string> BuildEmployerInterestDetailsList(EmployerInterest employerInterest, GeoLocation geoLocation)
    {
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

        var detailsList = new StringBuilder();
        detailsList.AppendLine($"* Name: {employerInterest.ContactName}");
        if (!string.IsNullOrEmpty(employerInterest.Email))
        {
            detailsList.AppendLine($"* Email address: {employerInterest.Email}");
        }
        if (!string.IsNullOrEmpty(employerInterest.Telephone))
        {
            detailsList.AppendLine($"* Telephone: {employerInterest.Telephone}");
        }

        if (employerInterest.ContactPreferenceType is not null)
        {
            var contactPreference = employerInterest.ContactPreferenceType switch
            {
                ContactPreference.Email => "Email",
                ContactPreference.Telephone => "Telephone",
                ContactPreference.NoPreference => "No preference",
                _ => "None"
            };

            detailsList.AppendLine($"* How would you prefer to be contacted: {contactPreference}");
        }

        detailsList.AppendLine($"* Organisation name: {employerInterest.OrganisationName}");

        if (!string.IsNullOrEmpty(employerInterest.Website))
        {
            detailsList.AppendLine($"* Website: {employerInterest.Website}");
        }

        detailsList.AppendLine($"* Organisation’s primary industry: {industry}");
        detailsList.AppendLine($"* Industry placement area{(skillAreas.Count > 1 ? "s" : "")}: {placementAreas}");
        detailsList.AppendLine($"* Postcode: {geoLocation.Location}");
        if (!string.IsNullOrEmpty(employerInterest.AdditionalInformation))
        {
            detailsList.AppendLine($"* Additional information: {employerInterest.AdditionalInformation.ReplaceMultipleLineBreaks()}");
        }

        return detailsList.ToString();
    }

    private async Task<GeoLocation> GetPostcode(string postcode)
    {
        var geoLocation = postcode.Length <= 4
            ? await _postcodeLookupService.GetOutcode(postcode)
            : await _postcodeLookupService.GetPostcode(postcode);

        if (geoLocation is null)
        {
            throw new PostcodeNotFoundException(postcode);
        }

        return geoLocation;
    }
}
