using NSubstitute;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;

public class EmailTemplateRepositoryBuilder
{
    public IEmailTemplateRepository Build(
        IDbContextWrapper dbContextWrapper = null,
        IDynamicParametersWrapper dynamicParametersWrapper = null)
    {
        dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
        dynamicParametersWrapper ??= Substitute.For<IDynamicParametersWrapper>();

        return new EmailTemplateRepository(
            dbContextWrapper,
            dynamicParametersWrapper);
    }

    public IEmailTemplateRepository BuildSubstitute(string templateId, string templateName)
    {
        var emailTemplateRepository = Substitute.For<IEmailTemplateRepository>();
        if (templateName != null)
        {
            emailTemplateRepository.GetEmailTemplate(templateId)
                .Returns(new EmailTemplate
                {
                    TemplateId = templateId,
                    Name = templateName
                });
            emailTemplateRepository.GetEmailTemplateByName(templateName)
                .Returns(new EmailTemplate
                {
                    TemplateId = templateId,
                    Name = templateName
                });
        }

        return emailTemplateRepository;
    }
}