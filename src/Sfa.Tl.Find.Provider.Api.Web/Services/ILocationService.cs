namespace Sfa.Tl.Find.Provider.Api.Web.Services;

public interface ILocationService
{
    Task<IEnumerable<LocationSearchResult>> Search(
        SearchTerms searchTerms,
        int maxResults = 50);
}