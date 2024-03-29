﻿using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface ISearchFilterRepository
{
    Task<IEnumerable<SearchFilter>> GetSearchFilterSummaryList(
        long ukPrn);

    Task<SearchFilter> GetSearchFilter(int locationId);

    Task Save(SearchFilter searchFilter);
}