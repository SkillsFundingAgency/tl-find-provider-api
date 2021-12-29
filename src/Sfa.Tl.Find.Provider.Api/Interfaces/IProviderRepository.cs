using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface IProviderRepository
{
    Task<bool> HasAny();

    Task Save(IList<Models.Provider> providers);

    Task<IEnumerable<ProviderSearchResult>> Search(
        PostcodeLocation fromPostcodeLocation,
        int? qualificationId,
        int page,
        int pageSize,
        bool mergeAdditionalProviderData);
}