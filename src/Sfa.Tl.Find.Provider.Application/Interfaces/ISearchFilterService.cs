using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface ISearchFilterService
{
    Task<IEnumerable<SearchFilter>> GetSearchFilterSummaryList(long ukPrn);

    Task<SearchFilter> GetSearchFilter(int locationId);

    Task SaveSearchFilter(SearchFilter searchFilter);
}
