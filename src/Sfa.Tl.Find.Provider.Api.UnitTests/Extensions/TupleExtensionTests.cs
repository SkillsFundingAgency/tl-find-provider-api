using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions;

public class TupleExtensionTests
{
    [Fact]
    public void ConvertToTuple_With_No_Changes_Returns_Expected_Results()
    {
        var changeResult =
            new DataBaseChangeResultBuilder()
                .Build();

        var results = changeResult.ConvertToTuple();

        results.Should().Be((0, 0, 0));
    }

    [Fact]
    public void ConvertToTuple_With_Inserts_Only_Returns_Expected_Results()
    {
        var changeResult =
            new DataBaseChangeResultBuilder()
                .WithInserts(1)
                .Build();

        var results = changeResult.ConvertToTuple();

        results.Should().Be((1, 0, 0));
    }

    [Fact]
    public void ConvertToTuple_With_Updates_Only_Returns_Expected_Results()
    {
        var changeResult =
            new DataBaseChangeResultBuilder()
                .WithUpdates(1)
                .Build();

        var results = changeResult.ConvertToTuple();

        results.Should().Be((0, 1, 0));
    }

    [Fact]
    public void ConvertToTuple_With_Deletes_Only_Returns_Expected_Results()
    {
        var changeResult = 
            new DataBaseChangeResultBuilder()
                .WithDeletes(1)
                .Build();

        var results = changeResult.ConvertToTuple();

        results.Should().Be((0, 0, 1));
    }

    [Fact]
    public void ConvertToTuple_With_All_Changes_Returns_Expected_Results()
    {
        var changeResult = new DataBaseChangeResultBuilder()
            .WithInserts(3)
            .WithUpdates(2)
            .WithDeletes(1)
            .Build();

        var results = changeResult.ConvertToTuple();

        results.Should().Be((3, 2, 1));
    }
}