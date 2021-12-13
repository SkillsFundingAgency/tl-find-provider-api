using System.Data;
using FluentAssertions;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Data;

public class DbContextWrapperTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(DbContextWrapper)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void CreateConnection_Returns_Expected_Connection()
    {
        var dbContextWrapper = new DbContextWrapperBuilder().Build();

        var connection = dbContextWrapper.CreateConnection();
        connection.Should().NotBeNull();
    }

    [Fact]
    public void BeginTransaction_Calls_Method_On_Connection()
    {
        var dbContextWrapper = new DbContextWrapperBuilder().Build();

        var connection = Substitute.For<IDbConnection>();
        dbContextWrapper.BeginTransaction(connection);

        connection.Received(1).BeginTransaction();
    }
}