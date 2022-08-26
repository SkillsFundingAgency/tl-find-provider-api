using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class IndexModelBuilder
{
    public IndexModel Build(
            IOptions<EmailSettings>? emailOptions = null,
            IEmailService? emailService = null,
            ILogger<IndexModel>? logger = null,
            PageContext? pageContext = null)
    {
        pageContext ??= new PageContextBuilder()
            .BuildPageContext();

        emailOptions ??= new SettingsBuilder()
            .BuildEmailSettings()
            .ToOptions();

        emailService ??= Substitute.For<IEmailService>();
        logger ??= Substitute.For<ILogger<IndexModel>>();

        //TODO: Add IndexModelBuilder
        var pageModel = new IndexModel(
            emailOptions,
            emailService,
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}
