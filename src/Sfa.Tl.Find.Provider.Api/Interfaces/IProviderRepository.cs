using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sfa.Tl.Find.Provider.Api.Interfaces
{
    public interface IProviderRepository
    {
        Task<IEnumerable<Models.Provider>> GetAll();

        //Task<IEnumerable<Models.Provider>> FindProviders(
        //    string postcode,
        //    int? qualificationId,
        //    int page,
        //    int pageSize);

        Task Save(IEnumerable<Models.Provider> providers);
    }
}
