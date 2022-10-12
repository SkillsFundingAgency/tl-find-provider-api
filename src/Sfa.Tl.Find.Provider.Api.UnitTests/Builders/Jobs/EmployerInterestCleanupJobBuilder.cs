using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Jobs;

public class EmployerInterestCleanupJobBuilder
{
    public EmployerInterestCleanupJob Build(
        IEmployerInterestService employerInterestService = null,
        ILogger<EmployerInterestCleanupJob> logger = null)
    {
        employerInterestService ??= Substitute.For<IEmployerInterestService>();
        logger ??= Substitute.For<ILogger<EmployerInterestCleanupJob>>();

        return new EmployerInterestCleanupJob(
            employerInterestService,
            logger);
    }
}