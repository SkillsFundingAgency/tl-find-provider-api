using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface IProviderRepository
{
    Task<bool> HasAny(bool isAdditionalData = false);

    Task Save(IList<Models.Provider> providers, bool isAdditionalData = false);

    Task<IEnumerable<ProviderSearchResult>> Search(
        PostcodeLocation fromPostcodeLocation,
        IList<int> routeIds,
        IList<int> qualificationIds,
        int page,
        int pageSize,
        bool mergeAdditionalData);
}