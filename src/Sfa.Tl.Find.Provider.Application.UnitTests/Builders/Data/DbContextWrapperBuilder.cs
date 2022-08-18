using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;

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
            var options = Substitute.For<IOptions<ConnectionStringSettings>>();
            options.Value.Returns(new ConnectionStringSettings
            {
                SqlConnectionString = connectionString
            });
            return options;
        }).Invoke();

        policyRegistry ??= Substitute.For<IReadOnlyPolicyRegistry<string>>();

        logger ??= Substitute.For<ILogger<DbContextWrapper>>();

        return new DbContextWrapper(connectionStringOptions, policyRegistry, logger);
    }

    public (IDbContextWrapper, IDbConnection) BuildSubstituteWrapperAndConnection()
    {
        var dbConnection = Substitute.For<IDbConnection>();

        var dbContextWrapper = Substitute.For<IDbContextWrapper>();
        dbContextWrapper
            .CreateConnection()
            .Returns(dbConnection);

        return (dbContextWrapper, dbConnection);
    }

    public (IDbContextWrapper, IDbConnection, IDbTransaction) BuildSubstituteWrapperAndConnectionWithTransaction()
    {
        var transaction = Substitute.For<IDbTransaction>();

        var (dbContextWrapper, dbConnection) = BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .BeginTransaction(dbConnection)
            .Returns(transaction);

        return (dbContextWrapper, dbConnection, transaction);
    }
}