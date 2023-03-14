using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public async Task UploadTowns_Processes_File()
    {
        var townDataService = Substitute.For<ITownDataService>();

        var controller = new DataImportControllerBuilder()
            .Build(townDataService: townDataService);

        await using var stream = await BuildTestCsvFileStream();
        var file = new FormFile(stream, 0, stream.Length, "test_form_file", "test.csv");

        var result = await controller.UploadTowns(file);

        var okResult = result as AcceptedResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(202);

        await townDataService
            .Received(1)
            .ImportTowns(Arg.Any<Stream>());
    }

    [Fact]
    public async Task UploadTowns_Processes_Zip_File()
    {
        var townDataService = Substitute.For<ITownDataService>();

        var controller = new DataImportControllerBuilder()
            .Build(townDataService: townDataService);

        await using var stream = await BuildTestCsvFileStream();
        var archive = new ZipArchiveBuilder()
            .Build("test.csv", stream);

        var file = new FormFile(archive, 0, archive.Length, "test_form_file", "test.zip");

        var result = await controller.UploadTowns(file);

        var okResult = result as AcceptedResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(202);

        await townDataService
            .Received(1)
            .ImportTowns(Arg.Any<Stream>());
    }
    
    [Fact]
    public async Task UploadTowns_For_Missing_File_Returns_Returns_BadRequest_Result()
    {
        var controller = new DataImportControllerBuilder()
            .Build();

        var result = await controller.UploadTowns(null);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult!.Value.Should().Be("File is required.");
    }

    [Fact]
    public async Task UploadTowns_For_Unsupported_File_Extension_Returns_Returns_BadRequest_Result()
    {
        var controller = new DataImportControllerBuilder()
            .Build();

        await using var stream = await "Test".ToStream();
        var file = new FormFile(stream, 0, stream.Length, "test_form_file", "test.txt");
        
        var result = await controller.UploadTowns(file);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult!.Value.Should().Be("Only csv or zip files are supported.");
    }

    private static async Task<Stream> BuildTestCsvFileStream(
         string content = "col1, col2\r\n123,Test")
    {
        return await content.ToStream();
    }
}
