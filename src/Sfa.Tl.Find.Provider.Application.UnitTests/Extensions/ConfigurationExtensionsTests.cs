﻿using Microsoft.Extensions.Configuration;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Extensions;
public class ConfigurationExtensionsTests
{
    [Fact]
    public void IsLocal_Returns_True_For_Local_Environment()
    {
        var configuration = Substitute.For<IConfiguration>();
        configuration[Constants.EnvironmentNameConfigKey]
            .Returns("LOCAL");

        configuration.IsLocal().Should().BeTrue();
    }

    [Fact]
    public void IsLocal_Returns_False_For_Remote_Environment()
    {
        var configuration = Substitute.For<IConfiguration>();
        configuration[Constants.EnvironmentNameConfigKey]
            .Returns("PROD");

        configuration.IsLocal().Should().BeFalse();
    }
}
