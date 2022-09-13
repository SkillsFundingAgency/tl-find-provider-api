using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;

public class EmployerInterestServiceBuilder
{
    public IEmployerInterestService Build(
        IDateTimeService dateTimeService = null,
        IEmployerInterestRepository? employerInterestRepository = null,
        EmployerInterestSettings employerInterestSettings = null,
        ILogger<EmployerInterestService> logger = null)
    {
        dateTimeService ??= Substitute.For<IDateTimeService>();
        employerInterestRepository ??= Substitute.For<IEmployerInterestRepository>();
        logger ??= Substitute.For<ILogger<EmployerInterestService>>();

        var employerInterestOptions = Options.Create(
            employerInterestSettings
            ?? new SettingsBuilder()
                .BuildEmployerInterestSettings());

        return new EmployerInterestService(
            dateTimeService,
            employerInterestRepository,
            employerInterestOptions,
            logger);
    }
}