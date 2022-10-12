using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders;
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
        var file = new FormFile(stream, 0, stream.Length, "test_form_file", "test.csv");

        var result = await controller.UploadProviderContacts(file);

        var okResult = result as AcceptedResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(202);

        await providerDataService
            .Received(1)
            .ImportProviderContacts(Arg.Any<Stream>());
    }

    [Fact]
    public async Task UploadProviderContacts_For_Missing_File_Returns_Returns_BadRequest_Result()
    {
        var controller = new DataImportControllerBuilder()
            .Build();

        var result = await controller.UploadProviderContacts(null);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult!.Value.Should().Be("File is required.");
    }

    [Fact]
    public async Task UploadProviderData_Processes_File()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new DataImportControllerBuilder()
            .Build(providerDataService);

        await using var stream = await BuildTestJsonFileStream();
        var file = new FormFile(stream, 0, stream.Length, "test_form_file", "test.json");

        var result = await controller.UploadProviderData(file);

        var okResult = result as AcceptedResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(202);

        await providerDataService
            .Received(1)
            .ImportProviderData(Arg.Any<Stream>(), true);
    }

    [Fact]
    public async Task UploadProviderData_For_Missing_File_Returns_Returns_BadRequest_Result()
    {
        var controller = new DataImportControllerBuilder()
            .Build();

        var result = await controller.UploadProviderData(null);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult!.Value.Should().Be("File is required.");
    }

    [Fact]
    public async Task UploadProviderDataZipped_Processes_File()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new DataImportControllerBuilder()
            .Build(providerDataService);

        await using var stream = await BuildTestJsonFileStream();
        var archive = new ZipArchiveBuilder()
            .Build("providerData.json", stream);

        var file = new FormFile(archive, 0, archive.Length, "test_form_file", "test.zip");

        var result = await controller.UploadProviderDataZipped(file);

        var okResult = result as AcceptedResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(202);

        await providerDataService
            .Received(1)
            .ImportProviderData(Arg.Any<Stream>(), true);
    }

    [Fact]
    public async Task UploadProviderDataZipped_With_No_Json_File_Returns_BadRequest_Result()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new DataImportControllerBuilder()
            .Build(providerDataService);

        await using var stream = await BuildTestCsvFileStream();
        var archive = new ZipArchiveBuilder()
            .Build("providerData.csv", stream);

        var file = new FormFile(archive, 0, archive.Length, "test_form_file", "test.zip");

        var result = await controller.UploadProviderDataZipped(file);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult!.Value.Should().Be("A zip file containing a json file is required.");

        await providerDataService
            .DidNotReceive()
            .ImportProviderData(Arg.Any<Stream>(), true);
    }

    [Fact]
    public async Task UploadProviderDataZipped_For_Missing_File_Returns_BadRequest_Result()
    {
        var controller = new DataImportControllerBuilder()
            .Build();

        var result = await controller.UploadProviderDataZipped(null);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult!.Value.Should().Be("File is required.");
    }

    private static async Task<Stream> BuildTestCsvFileStream(
         string content = "col1, col2\r\n123,Test")
    {
        return await content.ToStream();
    }

    private static async Task<Stream> BuildTestJsonFileStream(
        string content = "{ \"data\": \"test\" }")
    {
        return await content.ToStream();
    }
}
