using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Web.Pages;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages;
public class IndexPageTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(IndexModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task IndexModel_OnGet_Populates_Page_Properties()
    {
        var qualifications = new QualificationBuilder().BuildList();
        var qualificationRepository = Substitute.For<IQualificationRepository>();
        qualificationRepository.GetAll().Returns(qualifications);

        var emailOptions = new SettingsBuilder()
            .BuildEmailSettings()
            .ToOptions();

        var pageModel = new IndexModel(
            emailOptions,
            Substitute.For<IEmailService>(),
            qualificationRepository,
            Substitute.For<ILogger<IndexModel>>());

        await pageModel.OnGet();

        //TODO: Add tests
    }
}
