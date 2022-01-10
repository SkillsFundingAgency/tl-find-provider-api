using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface IQualificationRepository
{
    Task<bool> HasAny();

    Task<IEnumerable<Qualification>> GetAll();

    Task Save(IEnumerable<Qualification> qualifications);
}