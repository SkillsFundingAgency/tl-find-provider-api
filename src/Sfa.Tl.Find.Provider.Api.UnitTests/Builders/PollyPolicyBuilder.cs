using NSubstitute;
using Polly;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

public static class PollyPolicyBuilder
{
    public static (IAsyncPolicy, IReadOnlyPolicyRegistry<string>) BuildPolicyAndRegistry(
        IAsyncPolicy policy = null)
    {
        policy ??= Substitute.For<IAsyncPolicy>();
     
        var policyRegistry = Substitute.For<IReadOnlyPolicyRegistry<string>>();
        policyRegistry
            .Get<IAsyncPolicy>(Constants.DapperRetryPolicyName)
            .Returns(policy);

        return (policy, policyRegistry);
    }
}