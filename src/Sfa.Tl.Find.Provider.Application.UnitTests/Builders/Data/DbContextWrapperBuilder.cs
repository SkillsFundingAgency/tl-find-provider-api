using System.Data;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;

public class DbContextWrapperBuilder
{
    public DbContextWrapper Build(
        string connectionString = null,
        IReadOnlyPolicyRegistry<string> policyRegistry = null,
        ILogger<DbContextWrapper> logger = null)
    {
        var connectionStringSettings = connectionString is null 
            ? new SettingsBuilder().BuildConnectionStringSettings()
            : new SettingsBuilder().BuildConnectionStringSettings(connectionString);

        policyRegistry ??= Substitute.For<IReadOnlyPolicyRegistry<string>>();

        logger ??= Substitute.For<ILogger<DbContextWrapper>>();

        return new DbContextWrapper(
            connectionStringSettings.ToOptions(), 
            policyRegistry, 
            logger);
    }

    public IDbContextWrapper BuildSubstitute()
    {
        var dbConnection = Substitute.For<IDbConnection>();

        var dbContextWrapper = Substitute.For<IDbContextWrapper>();
        dbContextWrapper
            .CreateConnection()
            .Returns(dbConnection);

        return dbContextWrapper;
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