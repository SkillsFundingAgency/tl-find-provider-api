namespace Sfa.Tl.Find.Provider.Web.Authorization;

public static class CustomClaimTypes
{
    public const string AccessToken = "http://schemas.xmlsoap.org/ws/2009/09/identity/claims/accesstoken";

    public const string RefreshToken = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/refreshtoken";

    public const string HasAccessToService = "http://schemas.microsoft.com/ws/2008/06/identity/claims/hasaccesstoservice";

    public const string UkPrn = "http://schemas.microsoft.com/ws/2008/06/identity/claims/ukprn"; 

    public const string Urn = "http://schemas.microsoft.com/ws/2008/06/identity/claims/urn"; 

    public const string OrganisationId = "http://schemas.microsoft.com/ws/2008/06/identity/claims/organisationid";
    
    public const string OrganisationName = "http://schemas.microsoft.com/ws/2008/06/identity/claims/organisationname";

    public const string OrganisationCategory = "http://schemas.microsoft.com/ws/2008/06/identity/claims/organisationcategory";

    public const string UserId = "http://schemas.microsoft.com/ws/2008/06/identity/claims/userid";

    public const string LoginUserType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/loginusertype";



    public static string DisplayName => "http://schemas.portal.com/displayname";
    public static string Service => "http://schemas.portal.com/service";
    public static string Email => "http://schemas.portal.com/mail";
}