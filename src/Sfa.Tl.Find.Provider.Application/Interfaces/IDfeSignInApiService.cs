using Sfa.Tl.Find.Provider.Application.Models.Authentication;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IDfeSignInApiService
{
    Task<DfeUserInfo> GetDfeSignInUserInfo(string organisationId, string userId);
}