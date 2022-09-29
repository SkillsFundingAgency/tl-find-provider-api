﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Application.Services;
public class EmployerInterestService : IEmployerInterestService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IEmailService _emailService;
    private readonly IPostcodeLookupService _postcodeLookupService;
    private readonly IEmployerInterestRepository _employerInterestRepository;
    private readonly ILogger<EmployerInterestService> _logger;
    private readonly EmployerInterestSettings _employerInterestSettings;

    public EmployerInterestService(
        IDateTimeService dateTimeService,
        IEmailService emailService,
        IPostcodeLookupService postcodeLookupService,
        IEmployerInterestRepository employerInterestRepository,
        IOptions<EmployerInterestSettings> employerInterestOptions,
        ILogger<EmployerInterestService> logger)
    {
        _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _postcodeLookupService = postcodeLookupService ?? throw new ArgumentNullException(nameof(postcodeLookupService));
        _employerInterestRepository = employerInterestRepository ?? throw new ArgumentNullException(nameof(employerInterestRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _employerInterestSettings = employerInterestOptions?.Value
                                    ?? throw new ArgumentNullException(nameof(employerInterestOptions));
    }

    public async Task<Guid> CreateEmployerInterest(EmployerInterest employerInterest)
    {
        var (count, uniqueId) = await _employerInterestRepository.Create(employerInterest);

        if (uniqueId != Guid.Empty)
        {
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

        var unsubscribeUri = new Uri(new Uri(_employerInterestSettings.EmployerSupportSiteUri),
            employerInterest.UniqueId.ToString("D").ToLower());

        var tokens = new Dictionary<string, string>()
        {
            { "organisation_name", employerInterest.OrganisationName },
            { "contact_name", employerInterest.ContactName },
            { "email_address", employerInterest.Email },
            { "telephone", employerInterest.Telephone },
            //Need to convert to text 1=Email, 2=Telephone
            { "contact_preference", employerInterest.ContactPreferenceType.ToString() },
            //Need to convert to text
            { "primary_industry", employerInterest.IndustryId.ToString() },
            { "postcode", employerInterest.Postcode },
            { "specific_requirements", employerInterest.SpecificRequirements },
            //{ "how_many_locations", employerInterest.HasMultipleLocations + employerInterest.LocationCount }, //A single location
            { "how_many_locations", employerInterest.LocationCount.ToString() }, //A single location
            { "employer_support_site", _employerInterestSettings.EmployerSupportSiteUri },
            { "unique_id", employerInterest.UniqueId.ToString() },
            { "employer_unsubscribe_uri", unsubscribeUri.ToString() }
        };

        return await _emailService.SendEmail(
            employerInterest.Email,
            templateName,
            tokens);
    }
}
