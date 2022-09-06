using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Controllers;
public class DataImportControllerTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(DataImportController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(DataImportController)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task UploadProviderContacts_Processes_File()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new DataImportControllerBuilder()
            .Build(providerDataService);

        await using var stream = await BuildTestCsvFileStream();
        IFormFile file = new FormFile(stream, 0, stream.Length, "test_form_file", "test.csv");

        var result = await controller.UploadProviderContacts(file);

        var okResult = result as AcceptedResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(202);

        await providerDataService
            .Received(1)
            .ImportProviderContacts(Arg.Any<Stream>());
    }
    
    [Fact]
    public async Task UploadProviderContacts_Processes_File_With_Get_Method()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new DataImportControllerBuilder()
            .Build(providerDataService);

        await using var stream = await BuildTestCsvFileStream();
        IFormFile file = new FormFile(stream, 0, stream.Length, "test_form_file", "test.csv");

        var result = await controller.GetUploadProviderContacts(file);

        var okResult = result as AcceptedResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(202);

        await providerDataService
            .Received(1)
            .ImportProviderContacts(Arg.Any<Stream>());
    }

    [Fact]
    public async Task UploadProviderContacts_For_Missing_File_Returns_Error_Result()
    {
        var controller = new DataImportControllerBuilder()
            .Build();

        var result = await controller.UploadProviderContacts(null);

        var statusCodeResult = result as BadRequestObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(400);
        statusCodeResult!.Value.Should().Be("File is required.");
    }

    [Fact]
    public async Task UploadProviderContacts_For_Missing_File_Returns_Error_Result_With_Get_Method()
    {
        var controller = new DataImportControllerBuilder()
            .Build();

        var result = await controller.GetUploadProviderContacts(null);

        var statusCodeResult = result as BadRequestObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(400);
        statusCodeResult!.Value.Should().Be("File is required.");
    }

    private static async Task<Stream> BuildTestCsvFileStream(
        string content = "col1, col2\r\n123,Test")
    {
        var stream = new MemoryStream();

        await using var writer = new StreamWriter(stream, leaveOpen: true);
        await writer.WriteAsync(content);
        await writer.FlushAsync();
        stream.Position = 0;

        return stream;
    }
}
