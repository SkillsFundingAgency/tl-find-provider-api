﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface IProviderDataService
{
    Task<IEnumerable<Qualification>> GetQualifications();

    Task<IEnumerable<Route>> GetRoutes();

    Task<bool> HasQualifications();
    Task<bool> HasProviders();

    Task<ProviderSearchResponse> FindProviders(
        string postcode,
        int? qualificationId = null,
        int page = 0,
        int pageSize = Constants.DefaultPageSize);

    Task LoadAdditionalProviderData();
}