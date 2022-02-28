using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface IProviderReferenceDataService
{
    Task<List<ProviderReference>> GetAll(DateTime lastUpdateDate);

    Task<List<ProviderReference>> GetAllSinceLastUpdate();

    Task<bool> HasProviderReferences();

    Task Save(IEnumerable<ProviderReference> providerReferences);
}
