namespace Sfa.Tl.Find.Provider.Api.UnitTests.IntegrationTests;
public class TestConfigurationSettings
{
    public Guid EmployerInterestUniqueId { get; }

    public TestConfigurationSettings()
    {
        EmployerInterestUniqueId = Guid.NewGuid();
    }
}
