using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface IProviderReferenceRepository
{
    // ReSharper disable once UnusedMember.Global
    Task<ProviderReference> GetByUkprn(long ukprn);

    Task<bool> HasAny();

    // ReSharper disable once UnusedMember.Global
    Task<IEnumerable<ProviderReference>> GetAll();

    Task<DateTime?> GetLastUpdateDate();

    Task Save(IEnumerable<ProviderReference> providerReferences);
}