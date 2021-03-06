using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

        var fieldInfo = dynamicParametersWrapper.DynamicParameters.GetType()
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .SingleOrDefault(p => p.Name == "templates");

        fieldInfo.Should().NotBeNull();
        var templates = fieldInfo!.GetValue(dynamicParametersWrapper.DynamicParameters) as IList<object>;
        templates.Should().NotBeNullOrEmpty();
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
    public void AddParameter_Creates_Expected_Parameters()
    {
        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder().Build();

        dynamicParametersWrapper.AddParameter("bob", DbType.String);

        dynamicParametersWrapper.DynamicParameters.ParameterNames.Should().NotBeNullOrEmpty();
        dynamicParametersWrapper.DynamicParameters.ParameterNames
            .Should().Contain(s => s == "bob");
    }
}