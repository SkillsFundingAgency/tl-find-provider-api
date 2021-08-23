using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces
{
    public interface IQualificationRepository
    {
        Task<IEnumerable<Qualification>> GetAll();

        Task<(int Inserted, int Updated, int Deleted)> Save(IEnumerable<Qualification> qualifications);
    }
}
