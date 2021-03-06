using FluentAssertions;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Models;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions;

public class PollyRegistryExtensionsTests
{
    [Fact]
    public void PollyRegistryExtensions_AddDapperRetryPolicy_Succeeds()
    {
        var pollyPolicyRegistry = new PolicyRegistry();

        var result = pollyPolicyRegistry.AddDapperRetryPolicy();

        result.Should().BeSameAs(pollyPolicyRegistry);  

        pollyPolicyRegistry.ContainsKey(Constants.DapperRetryPolicyName).Should().BeTrue();

        var d = pollyPolicyRegistry[Constants.DapperRetryPolicyName];
        d.PolicyKey.Should().Be(Constants.DapperRetryPolicyName);
    }
}