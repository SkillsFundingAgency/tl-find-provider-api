using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IProviderRepository
{
    Task<IEnumerable<ProviderDetail>> GetAll();

    Task<IEnumerable<ProviderDetailFlat>> GetAllFlattened();

    Task<IEnumerable<LocationPostcode>> GetLocationPostcodes(long ukPrn);

    Task<bool> HasAny();

    Task Save(IList<Models.Provider> providers);

    Task<(IEnumerable<ProviderSearchResult> SearchResults, int TotalResultsCount)> Search(
        GeoLocation fromGeoLocation,
        IList<int> routeIds,
        IList<int> qualificationIds,
        int page,
        int pageSize);
}