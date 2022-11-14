using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;

public class EmployerInterestServiceBuilder
{
    public IEmployerInterestService Build(
        IDateTimeService dateTimeService = null,
        IEmailService emailService = null,
        IPostcodeLookupService postcodeLookupService = null,
        IProviderDataService providerDataService = null,
        IEmployerInterestRepository employerInterestRepository = null,
        EmployerInterestSettings employerInterestSettings = null,
        ILogger<EmployerInterestService> logger = null)
    {
        dateTimeService ??= Substitute.For<IDateTimeService>();
        emailService ??= Substitute.For<IEmailService>();
        postcodeLookupService ??= Substitute.For<IPostcodeLookupService>();
        employerInterestRepository ??= Substitute.For<IEmployerInterestRepository>();
        providerDataService ??= Substitute.For<IProviderDataService>();
        logger ??= Substitute.For<ILogger<EmployerInterestService>>();

        var employerInterestOptions = Options.Create(
            employerInterestSettings
            ?? new SettingsBuilder()
                .BuildEmployerInterestSettings());

        return new EmployerInterestService(
            dateTimeService,
            emailService,
            postcodeLookupService,
            providerDataService,
            employerInterestRepository,
            employerInterestOptions,
            logger);
    }
}