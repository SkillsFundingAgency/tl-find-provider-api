﻿using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IProviderRepository
{
    Task<bool> HasAny(bool isAdditionalData = false);

    Task Save(IList<Models.Provider> providers, bool isAdditionalData = false);

    Task<IEnumerable<ProviderSearchResult>> GetAllProviderResults();

    Task<(IEnumerable<ProviderSearchResult> SearchResults, int TotalResultsCount)> Search(
        GeoLocation fromGeoLocation,
        IList<int> routeIds,
        IList<int> qualificationIds,
        int page,
        int pageSize,
        bool includeAdditionalData);
}