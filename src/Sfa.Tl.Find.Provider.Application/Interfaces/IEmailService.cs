
namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmail(
        string recipient,
        string templateName, 
        IDictionary<string, string> tokens);

    Task<bool> SendEmail(
        string recipient,
        string templateName,
        Dictionary<string, dynamic> tokens);
}
