using System.Data;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Data;

public class DynamicParametersWrapperTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(DynamicParametersWrapper)
            .ShouldNotAcceptNullConstructorArguments();
    }
    
    [Fact]
    public void Constructor_Sets_Dynamic_Parameters_Property()
    {
        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder().Build();

        dynamicParametersWrapper.DynamicParameters.Should().NotBeNull();
    }

    [Fact]
    public void CreateParameters_From_Key_Value_Pairs_Creates_Expected_Parameters()
    {
        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder().Build();

        var kvp = new List<KeyValuePair<string, object>>
        {
            new("id", 10),
            new("name", "test")
        };

        dynamicParametersWrapper.CreateParameters(kvp);

        dynamicParametersWrapper.DynamicParameters.ParameterNames.Should().NotBeNullOrEmpty();
        dynamicParametersWrapper.DynamicParameters.ParameterNames
            .Should().Contain(s => s == "id");
        dynamicParametersWrapper.DynamicParameters.ParameterNames
           .Should().Contain(s => s == "name");
    }

    [Fact]
    public void CreateParameters_From_Anonymous_Template_Creates_Expected_Parameters()
    {
        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder().Build();

        var obj = new
        {
            id = 10,
            name = "test"
        };

        dynamicParametersWrapper.CreateParameters(obj);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
         templates!.First().Should().Be(obj);
    }

    [Fact]
    public void AddOutputParameter_Creates_Expected_Parameters()
    {
        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder().Build();

        dynamicParametersWrapper.AddOutputParameter("bob", DbType.String);

        dynamicParametersWrapper.DynamicParameters.ParameterNames.Should().NotBeNullOrEmpty();
        dynamicParametersWrapper.DynamicParameters.ParameterNames
            .Should().Contain(s => s == "bob");
    }

    [Fact]
    public void AddReturnValueParameter_Creates_Expected_Parameters()
    {
        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder().Build();

        dynamicParametersWrapper.AddReturnValueParameter("bob", DbType.String);

        dynamicParametersWrapper.DynamicParameters.ParameterNames.Should().NotBeNullOrEmpty();
        dynamicParametersWrapper.DynamicParameters.ParameterNames
            .Should().Contain(s => s == "bob");
    }

    [Fact]
    public void AddParameter_Creates_Expected_Parameters()
    {
        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder().Build();

        dynamicParametersWrapper.AddParameter("bob", DbType.String);

        dynamicParametersWrapper.DynamicParameters.ParameterNames.Should().NotBeNullOrEmpty();
        dynamicParametersWrapper.DynamicParameters.ParameterNames
            .Should().Contain(s => s == "bob");
    }
}