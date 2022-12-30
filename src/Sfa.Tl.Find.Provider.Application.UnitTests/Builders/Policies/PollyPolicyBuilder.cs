using Notify.Models.Responses;
using Polly;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Policies;

public static class PollyPolicyBuilder
{
    public static IAsyncPolicy BuildPolicy()
    {
        var policy = Substitute.For<IAsyncPolicy>();
        policy
            .When(x =>
                x.ExecuteAsync(
                    Arg.Any<Func<Context, Task>>(),
                    Arg.Any<Context>()))
            .Do(x =>
            {
                var func = x.Arg<Func<Context, Task>>();
                var context = x.Arg<Context>();
                func.Invoke(context);
            });
        
        return policy;
    }

    public static IAsyncPolicy BuildPolicy<TResult>()
    {
        var policy = Substitute.For<IAsyncPolicy>();
        policy
            .ExecuteAsync(
                Arg.Any<Func<Context, Task<TResult>>>(),
                Arg.Any<Context>())
            .Returns(x =>
            {
                var func = x.Arg<Func<Context, Task<TResult>>>();
                var context = x.Arg<Context>();
                var r = func.Invoke(context);
                return r.Result;
            });
        return policy;
    }

    public static IReadOnlyPolicyRegistry<string> BuildPolicyRegistry(
        IAsyncPolicy policy,
        string policyKey = Constants.DapperRetryPolicyName)
    {
        var policyRegistry = Substitute.For<IReadOnlyPolicyRegistry<string>>();
        policyRegistry
            .Get<IAsyncPolicy>(policyKey)
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

    public static (IAsyncPolicy, IReadOnlyPolicyRegistry<string>) BuildDapperPolicyAndRegistry(
        IAsyncPolicy policy = null)
    {
        policy ??= BuildPolicy();

        var policyRegistry = BuildPolicyRegistry(policy);

        return (policy, policyRegistry);
    }

    public static (IAsyncPolicy Policy, IReadOnlyPolicyRegistry<string> Registry) BuildGovNotifyPolicyAndRegistry(
        IAsyncPolicy policy = null)
    {
        policy ??= BuildPolicy<EmailNotificationResponse>();
        var pollyPolicyRegistry = BuildPolicyRegistry(
            policy,
            policyKey: Constants.GovNotifyRetryPolicyName);

        return (policy, pollyPolicyRegistry);
    }
}