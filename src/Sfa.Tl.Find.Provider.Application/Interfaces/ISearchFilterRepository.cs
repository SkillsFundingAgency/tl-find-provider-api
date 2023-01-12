using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface ISearchFilterRepository
{
    //Task Delete(int id);

    //Task Save(SearchFilter searchFilter);

    Task<IEnumerable<SearchFilter>> GetSearchFilters(
        long ukPrn,
        bool includeAdditionalData);

    Task<SearchFilter> GetSearchFilter(int locationId);
}