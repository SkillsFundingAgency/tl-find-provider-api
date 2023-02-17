namespace Sfa.Tl.Find.Provider.Web.Authorization;

public static class PolicyNames
{
    public static string HasProviderAccount => nameof(HasProviderAccount);
    public static string IsAdministrator => nameof(IsAdministrator);
    public static string IsProviderOrAdministrator => nameof(IsProviderOrAdministrator);
}