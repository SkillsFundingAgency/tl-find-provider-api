using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IProviderDataService
{
    Task<IEnumerable<Qualification>> GetQualifications();

    Task<IEnumerable<Route>> GetRoutes();

    Task<bool> HasQualifications();
    Task<bool> HasProviders();

    Task<ProviderSearchResponse> FindProviders(
        string searchTerms,
        IList<int> routeIds = null,
        IList<int> qualificationIds = null,
        int page = 0,
        int pageSize = Constants.DefaultPageSize);

    Task<ProviderSearchResponse> FindProviders(
        double latitude,
        double longitude,
        IList<int> routeIds = null,
        IList<int> qualificationIds = null,
        int page = 0,
        int pageSize = Constants.DefaultPageSize);

    Task<ProviderDetailResponse> GetAllProviders();

    Task ImportProviderData(Stream stream, bool isAdditionalData);

    Task<byte[]> GetCsv();

    Task ImportProviderContacts(Stream stream);
}