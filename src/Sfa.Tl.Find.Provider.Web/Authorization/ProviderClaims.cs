namespace Sfa.Tl.Find.Provider.Web.Authorization;

public static class ProviderClaims
{
    public static string ProviderUkprn => "http://schemas.portal.com/ukprn";
    public static string DisplayName => "http://schemas.portal.com/displayname";
    public static string Service => "http://schemas.portal.com/service";
    public static string UserId => "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";
    public static string Email => "http://schemas.portal.com/mail";
}