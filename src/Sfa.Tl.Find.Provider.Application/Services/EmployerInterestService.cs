using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Application.Services;
public class EmployerInterestService : IEmployerInterestService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IEmployerInterestRepository _employerInterestRepository;
    private readonly ILogger<EmployerInterestService> _logger;
    private readonly EmployerInterestSettings _employerInterestSettings;

    public EmployerInterestService(
        IDateTimeService dateTimeService,
        IEmployerInterestRepository employerInterestRepository,
        IOptions<EmployerInterestSettings> employerInterestOptions,
        ILogger<EmployerInterestService> logger)
    {
        _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        _employerInterestRepository = employerInterestRepository ?? throw new ArgumentNullException(nameof(employerInterestRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _employerInterestSettings = employerInterestOptions?.Value
                                    ?? throw new ArgumentNullException(nameof(employerInterestOptions));
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

        _logger.LogInformation("Removed {count} employer interest records because they are over {days} old (Date: {date}",
            count, _employerInterestSettings.RetentionDays, date);

        return count;
    }
}
