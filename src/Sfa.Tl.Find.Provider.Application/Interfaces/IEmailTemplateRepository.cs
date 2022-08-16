using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IEmailTemplateRepository
{
    Task<EmailTemplate> GetEmailTemplate(string templateName);
}