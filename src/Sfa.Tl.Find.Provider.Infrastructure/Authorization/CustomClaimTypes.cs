namespace Sfa.Tl.Find.Provider.Infrastructure.Authorization;

public static class CustomClaimTypes
{
    public const string AccessToken = "http://schemas.xmlsoap.org/ws/2009/09/identity/claims/accesstoken";

    public const string RefreshToken = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/refreshtoken";

    public const string UkPrn = "ukprn";

    public const string Urn = "urn";

    public const string OrganisationId = "org_id";

    public const string OrganisationName = "org_name";

    public const string OrganisationCategory = "org_category";

    public const string UserId = "user_id";

    public const string LoginUserType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/loginusertype";

    public static string DisplayName => "http://schemas.portal.com/displayname";
}