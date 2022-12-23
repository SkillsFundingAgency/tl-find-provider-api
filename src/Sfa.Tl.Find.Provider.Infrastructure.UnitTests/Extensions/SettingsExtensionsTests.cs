using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

namespace Sfa.Tl.Find.Provider.Infrastructure.UnitTests.Extensions;
public class SettingsExtensionsTests
{
    [Fact]
    public void ConfigureApiSettings_Returns_Expected_Value()
    {
        var configuration = new SettingsBuilder().BuildConfigurationOptions();
        var expectedSettings = new SettingsBuilder().BuildApiSettings();

        var targetSettings = new ApiSettings();
        targetSettings.ConfigureApiSettings(configuration);

        targetSettings.Should().BeEquivalentTo(expectedSettings);
    }

    [Fact]
    public void ConfigureConnectionStringSettings_Returns_Expected_Value()
    {
        var configuration = new SettingsBuilder().BuildConfigurationOptions();
        var expectedSettings = new SettingsBuilder().BuildConnectionStringSettings();

        var targetSettings = new ConnectionStringSettings();
        targetSettings.ConfigureConnectionStringSettings(configuration);

        targetSettings.Should().BeEquivalentTo(expectedSettings);
    }

    [Fact]
    public void ConfigureCourseDirectoryApiSettings_Returns_Expected_Value()
    {
        var configuration = new SettingsBuilder().BuildConfigurationOptions();
        var expectedSettings = new SettingsBuilder().BuildCourseDirectoryApiSettings();

        var targetSettings = new CourseDirectoryApiSettings();
        targetSettings.ConfigureCourseDirectoryApiSettings(configuration);

        targetSettings.Should().BeEquivalentTo(expectedSettings);
    }

    [Fact]
    public void ConfigureDfeSignInSettings_Returns_Expected_Value()
    {
        var configuration = new SettingsBuilder().BuildConfigurationOptions();
        var expectedSettings = new SettingsBuilder().BuildDfeSignInSettings();

        var targetSettings = new DfeSignInSettings();
        targetSettings.ConfigureDfeSignInSettings(configuration);

        targetSettings.Should().BeEquivalentTo(expectedSettings);
    }

    [Fact]
    public void ConfigureEmailSettings_Returns_Expected_Value()
    {
        var configuration = new SettingsBuilder().BuildConfigurationOptions();
        var expectedSettings = new SettingsBuilder().BuildEmailSettings();

        var targetSettings = new EmailSettings();
        targetSettings.ConfigureEmailSettings(configuration);

        targetSettings.Should().BeEquivalentTo(expectedSettings);
    }

    [Fact]
    public void ConfigureEmployerInterestSettings_Returns_Expected_Value()
    {
        var configuration = new SettingsBuilder().BuildConfigurationOptions();
        var expectedSettings = new SettingsBuilder().BuildEmployerInterestSettings();

        var targetSettings = new EmployerInterestSettings();
        targetSettings.ConfigureEmployerInterestSettings(configuration);

        targetSettings.Should().BeEquivalentTo(expectedSettings);
    }

    [Fact]
    public void ConfigureGoogleMapsApiSettings_Returns_Expected_Value()
    {
        var configuration = new SettingsBuilder().BuildConfigurationOptions();
        var expectedSettings = new SettingsBuilder().BuildGoogleMapsApiSettings();

        var targetSettings = new GoogleMapsApiSettings();
        targetSettings.ConfigureGoogleMapsApiSettings(configuration);

        targetSettings.Should().BeEquivalentTo(expectedSettings);
    }

    [Fact]
    public void ConfigurePostcodeApiSettings_Returns_Expected_Value()
    {
        var configuration = new SettingsBuilder().BuildConfigurationOptions();
        var expectedSettings = new SettingsBuilder().BuildPostcodeApiSettings();

        var targetSettings = new PostcodeApiSettings();
        targetSettings.ConfigurePostcodeApiSettings(configuration);

        targetSettings.Should().BeEquivalentTo(expectedSettings);
    }

    [Fact]
    public void ConfigureSearchSettings_Returns_Expected_Value()
    {
        var configuration = new SettingsBuilder().BuildConfigurationOptions();
        var expectedSettings = new SettingsBuilder().BuildSearchSettings();

        var targetSettings = new SearchSettings();
        targetSettings.ConfigureSearchSettings(configuration);

        targetSettings.Should().BeEquivalentTo(expectedSettings);
    }
}
