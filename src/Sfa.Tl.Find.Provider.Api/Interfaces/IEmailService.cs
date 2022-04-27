using System.Threading.Tasks;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface IEmailService
{
    public Task<bool> SendEmployerInterestEmail(
        string employerName,
        string employerTelephone,
        string employerEmail,
        string providers);
}
