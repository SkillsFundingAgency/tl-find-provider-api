namespace Sfa.Tl.Find.Provider.Web.Authorization;

public static class PolicyNames
{
    public static string IsAdministrator => nameof(IsAdministrator);

    public static string IsProvider => nameof(IsProvider);
    
    public static string IsProviderOrAdministrator => nameof(IsProviderOrAdministrator);
}