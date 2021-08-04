using System.Linq;
using System.Threading.Tasks;

namespace Sfa.Tl.Find.Provider.Api.Interfaces
{
    public interface IProviderRepository
    {
        Task<IQueryable<Models.Provider>> GetAllProviders();

        //Task<IQueryable<Models.Provider>> FindProviders(
        //    string postCode,
        //    int? qualificationId,
        //    int page,
        //    int pageSize);
    }
}
