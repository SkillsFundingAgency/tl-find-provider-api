using Sfa.Tl.Find.Provider.Application.Models.Authentication;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IDfeSignInApiService
{
    Task<(DfeOrganisationInfo OrganisationInfo, DfeUserInfo UserInfo)> GetDfeSignInInfo(string organisationId, string userId);
}