using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

public class DbContextWrapperBuilder
{
    public DbContextWrapper Build(
        string connectionString = null,
        IReadOnlyPolicyRegistry<string> policyRegistry = null,
        ILogger<DbContextWrapper> logger = null)
    {
        connectionString ??= "Data Source=Test;Initial Catalog=Test;Integrated Security=True;";

        var connectionStringOptions = new Func<IOptions<ConnectionStringSettings>>(() =>
        {
            var config = Substitute.For<IOptions<ConnectionStringSettings>>();
            config.Value.Returns(new ConnectionStringSettings
            {
                SqlConnectionString = connectionString
            });
            return config;
        }).Invoke();

        policyRegistry ??= Substitute.For<IReadOnlyPolicyRegistry<string>>();

        logger ??= Substitute.For<ILogger<DbContextWrapper>>();

        return new DbContextWrapper(connectionStringOptions, policyRegistry, logger);
    }
}