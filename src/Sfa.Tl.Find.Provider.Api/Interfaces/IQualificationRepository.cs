using System.Linq;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces
{
    public interface IQualificationRepository
    {
        Task<IQueryable<Qualification>> GetAllQualifications();
    }
}
