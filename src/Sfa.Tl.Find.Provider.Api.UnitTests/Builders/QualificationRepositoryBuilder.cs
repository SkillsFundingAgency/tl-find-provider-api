using Microsoft.Extensions.Logging;
using NSubstitute;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

public class QualificationRepositoryBuilder
{
    public QualificationRepository Build(
        IDbContextWrapper dbContextWrapper = null,
        IReadOnlyPolicyRegistry<string> policyRegistry = null,
        ILogger<QualificationRepository> logger = null)
    {
        dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
        policyRegistry ??= Substitute.For<IReadOnlyPolicyRegistry<string>>();
        logger ??= Substitute.For<ILogger<QualificationRepository>>();

        return new QualificationRepository(
            dbContextWrapper,
            policyRegistry,
            logger);
    }
}