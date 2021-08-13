using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces
{
    public interface IProviderRepository
    {
        Task<(int Inserted, int Updated, int Deleted)> Save(IEnumerable<Models.Provider> providers);

        Task<IEnumerable<Models.Provider>> Search(
            PostcodeLocation fromPostcodeLocation,
            int? qualificationId,
            int page,
            int pageSize);
    }
}
