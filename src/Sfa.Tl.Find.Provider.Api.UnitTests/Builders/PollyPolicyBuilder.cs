using System;
using System.Threading.Tasks;
using NSubstitute;
using Polly;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

public static class PollyPolicyBuilder
{
    public static IAsyncPolicy BuildPolicy()
    {
        var policy = Substitute.For<IAsyncPolicy>();

        policy
            .When(x =>
            x.ExecuteAsync(
                Arg.Any<Func<Context, Task>>(),
                Arg.Any<Context>()
            ))
            .Do(x =>
            {
                var func = x.Arg<Func<Context, Task>>();
                var context = x.Arg<Context>();
                func.Invoke(context);
            });

        return policy;
    }

    public static IReadOnlyPolicyRegistry<string> BuildPolicyRegistry(
        IAsyncPolicy policy)
    {
        var policyRegistry = Substitute.For<IReadOnlyPolicyRegistry<string>>();
        policyRegistry
            .Get<IAsyncPolicy>(Constants.DapperRetryPolicyName)
            .Returns(policy);

        return policyRegistry;
    }

    public static (IAsyncPolicy, IReadOnlyPolicyRegistry<string>) BuildPolicyAndRegistry(
        IAsyncPolicy policy = null)
    {
        policy ??= Substitute.For<IAsyncPolicy>();

        var policyRegistry = BuildPolicyRegistry(policy);

        return (policy, policyRegistry);
    }
}