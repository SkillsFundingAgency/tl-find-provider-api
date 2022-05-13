using System.Data;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Data;

public class DynamicParametersWrapperTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(DynamicParametersWrapper)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void CreateConnection_Returns_Expected_Connection()
    {
        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder().Build();

        var obj = new
        {
            name = "test"
        };

        dynamicParametersWrapper.CreateParameters(obj);

        dynamicParametersWrapper.AddOutputParameter("bob", DbType.String);
        dynamicParametersWrapper.DynamicParameters.Should().NotBeNull();
        dynamicParametersWrapper.DynamicParameters.ParameterNames.Should().NotBeNullOrEmpty();
        dynamicParametersWrapper.DynamicParameters.ParameterNames
            .Should().Contain(s => s == "bob");

        //var b = dynamicParametersWrapper
        //    .DynamicParameters
        //    .Get<string>("Bob");
        //dynamicParametersWrapper
        //    .DynamicParameters
        //    .Get<string>("Bob")
        //    .Should().Be("test");
    }
}