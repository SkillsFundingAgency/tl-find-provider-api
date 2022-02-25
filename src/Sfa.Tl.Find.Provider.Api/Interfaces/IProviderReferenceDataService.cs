using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface IProviderReferenceDataService
{
    Task<List<ProviderReference>> GetAllAsync(DateTime lastUpdateDate);
}
