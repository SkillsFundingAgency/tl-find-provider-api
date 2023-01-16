using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface ISearchFilterRepository
{
    Task<IEnumerable<SearchFilter>> GetSearchFilters(
        long ukPrn,
        bool includeAdditionalData);

    Task<SearchFilter> GetSearchFilter(int locationId);

    Task Save(SearchFilter searchFilter);
}