using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Services;

public class TownDataService : ITownDataService
{
    private readonly HttpClient _httpClient;
    private readonly ITownRepository _townRepository;
    private readonly ILogger<TownDataService> _logger;

    public const string NationalOfficeOfStatisticsLocationUrl = "https://services1.arcgis.com/ESMARspQHYMw9BZ9/arcgis/rest/services/IPN_GB_2016/FeatureServer/0/query?where=ctry15nm%20%3D%20'ENGLAND'%20AND%20popcnt%20%3E%3D%20500%20AND%20popcnt%20%3C%3D%2010000000&outFields=placeid,place15nm,ctry15nm,cty15nm,ctyltnm,lad15nm,laddescnm,lat,long,descnm&returnDistinctValues=true&outSR=4326&f=json";

    public TownDataService(
        HttpClient httpClient,
        ITownRepository townRepository,
        ILogger<TownDataService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _townRepository = townRepository ?? throw new ArgumentNullException(nameof(townRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HasTowns()
    {
        return await _townRepository.HasAny();
    }

    public async Task ImportTowns()
    {
        var offSet = 0;
        const int recordSize = 2000;
        var moreData = true;

        var featureItems = new List<LocationApiItem>();

        while (moreData)
        {
            var uri = GetUri(offSet, recordSize);
            var responseMessage = await _httpClient.GetAsync(uri);

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError("National Statistics API call to '{uri}' failed with " +
                                 "{StatusCode} - {ReasonPhrase}",
                    uri, responseMessage.StatusCode, responseMessage.ReasonPhrase);
            }

            responseMessage.EnsureSuccessStatusCode();

            (var items, moreData) = await ReadLocationApiDataResponse(
                responseMessage);

            offSet += recordSize;

            featureItems.AddRange(items);
        }

        var towns = featureItems
            .Where(item => !string.IsNullOrEmpty(item.Name) &&
                           !string.IsNullOrEmpty(item.LocalAuthorityName) &&
                           !string.IsNullOrEmpty(item.LocalAuthorityDistrict) &&
                            item.PlaceNameDescription != "None")
            .GroupBy(c => new
            {
                c.LocalAuthorityName,
                c.Name,
                c.LocalAuthorityDistrict
            })
            .Select(item => item.First())
            .GroupBy(c => new
            {
                c.Id
            })
            .Select(SelectDuplicateByLocalAuthorityDistrictDescription)
            .Select(item => new Town
            {
                Id = item.Id,
                Name = item.Name,
                County = item.County,
                LocalAuthorityName = item.LocalAuthorityName,
                Latitude = item.Latitude,
                Longitude = item.Longitude
            })
            .ToList();

        await _townRepository.Save(towns);
    }

    public async Task<IEnumerable<Town>> Search(string searchTerm, int maxResults = Constants.TownSearchDefaultMaxResults)
    {
        if (searchTerm.IsFullOrPartialPostcode())
        {
            return new List<Town>();
        }

        return await _townRepository.Search(searchTerm, maxResults);
    }

    public Uri GetUri(int offset, int recordSize) =>
        new($"{NationalOfficeOfStatisticsLocationUrl}&resultRecordCount={recordSize}&resultOffSet={offset}");

    private static async Task<(IEnumerable<LocationApiItem>, bool)> ReadLocationApiDataResponse(
        HttpResponseMessage responseMessage)
    {
        var jsonDocument = await JsonDocument.ParseAsync(await responseMessage.Content.ReadAsStreamAsync());
        //var json = jsonDocument.PrettifyJson();

        var root = jsonDocument.RootElement;

        var exceededTransferLimit = root
                                        .TryGetProperty("exceededTransferLimit", out var property)
                                    && property.GetBoolean();

        var towns = //new List<LocationApiItem>();
            root
            .GetProperty("features")
            .EnumerateArray()
            .Select(attr => new { attributeElement = attr.GetProperty("attributes") })
            .Select(x => new LocationApiItem
            {
                // ReSharper disable StringLiteralTypo
                Id = x.attributeElement.SafeGetInt32("placeid"),
                Name = x.attributeElement.SafeGetString("place15nm", Constants.LocationNameMaxLength),
                County = x.attributeElement.SafeGetString("cty15nm", Constants.CountyMaxLength),
                LocalAuthorityName = x.attributeElement.SafeGetString("ctyltnm", Constants.CountyMaxLength),
                LocalAuthorityDistrict = x.attributeElement.SafeGetString("lad15nm"),
                LocalAuthorityDistrictDescription = x.attributeElement.SafeGetString("laddescnm"),
                PlaceNameDescription = x.attributeElement.SafeGetString("descnm"),
                Latitude = x.attributeElement.SafeGetDecimal("lat"),
                Longitude = x.attributeElement.SafeGetDecimal("long")
                //ReSharper restore StringLiteralTypo
            });

        return (towns, exceededTransferLimit);
    }

    private static LocationApiItem SelectDuplicateByLocalAuthorityDistrictDescription(IEnumerable<LocationApiItem> items)
    {
        var values = items.ToList();

        if (values.Count > 1)
        {
            var orderByDescending = values.OrderByDescending(c => c.LocalAuthorityDistrict).ToList();
            var locationApiItem = orderByDescending.FirstOrDefault(c => !string.IsNullOrEmpty(c.LocalAuthorityDistrictDescription)
                                                                        && c.Name.Equals(c.LocalAuthorityDistrict, StringComparison.CurrentCultureIgnoreCase));
            if (locationApiItem != null)
            {
                return locationApiItem;
            }
            return orderByDescending
                .FirstOrDefault(c => !string.IsNullOrEmpty(c.LocalAuthorityDistrictDescription));
        }

        return values.FirstOrDefault();
    }
}