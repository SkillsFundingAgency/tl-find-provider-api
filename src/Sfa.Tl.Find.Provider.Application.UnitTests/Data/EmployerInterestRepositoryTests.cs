using System.Data;
using FluentAssertions;
using NSubstitute;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Data;

public class EmployerInterestRepositoryTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(EmployerInterestRepository)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task Delete_Returns_Expected_Result()
    {
        const int count = 1;
        var uniqueId = Guid.Parse("5FBDFA5D-3987-4A3D-B4A2-DBAF545455CB");

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s => 
                    s.Contains("DELETE dbo.EmployerInterest WHERE UniqueId = @uniqueId")),
                Arg.Any<object>(),
                Arg.Is<IDbTransaction>(t => t != null))
        .Returns(count);

        var repository = new EmployerInterestRepositoryBuilder().Build(dbContextWrapper);

        var result = await repository.Delete(uniqueId);

        result.Should().Be(count);
    }
    
    [Fact]
    public async Task DeleteBefore_Returns_Expected_Result()
    {
        const int count = 10;
        var date = DateTime.Parse("2022-09-13");

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("DELETE dbo.EmployerInterest WHERE CreatedOn < @date")),
                Arg.Any<object>(),
                Arg.Is<IDbTransaction>(t => t != null))
            .Returns(count);

        var repository = new EmployerInterestRepositoryBuilder().Build(dbContextWrapper);

        var result = await repository.DeleteBefore(date);

        result.Should().Be(count);
    }
}