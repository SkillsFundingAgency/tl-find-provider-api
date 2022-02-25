using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface IProviderReferenceRepository
{
    Task<ProviderReference> GetByUkPrn(long ukprn);

    Task<bool> HasAny();

    Task<IEnumerable<ProviderReference>> GetAll();

    Task Save(IEnumerable<ProviderReference> providerReferences);
}