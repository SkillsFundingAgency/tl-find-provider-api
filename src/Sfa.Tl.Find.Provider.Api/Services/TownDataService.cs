using System;
using System.Collections.Generic;
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

    public const string NationalOfficeOfStatisticsLocationUrl = "https://services1.arcgis.com/ESMARspQHYMw9BZ9/arcgis/rest/services/IPN_GB_2016/FeatureServer/0/query?where=ctry15nm%20%3D%20'ENGLAND'%20AND%20popcnt%20%3E%3D%20500%20AND%20popcnt%20%3C%3D%2010000000&outFields=placeid,place15nm,ctry15nm,cty15nm,ctyltnm,lat,long&returnDistinctValues=true&outSR=4326&f=json";

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

        var towns = new List<Town>();

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

            (var items, moreData) = await ReadTownDataResponse(
                responseMessage);

            offSet += recordSize;

            towns.AddRange(items);
        }

        await _townRepository.Save(towns);
    }

    public Uri GetUri(int offset, int recordSize) =>
        new($"{NationalOfficeOfStatisticsLocationUrl}&resultRecordCount={recordSize}&resultOffSet={offset}");

    private static async Task<(List<Town>, bool)> ReadTownDataResponse(
            HttpResponseMessage responseMessage)
    {
        var jsonDocument = await JsonDocument.ParseAsync(await responseMessage.Content.ReadAsStreamAsync());

        var root = jsonDocument.RootElement;

        var exceededTransferLimit = root
            .TryGetProperty("exceededTransferLimit", out var property)
                && property.GetBoolean();

        var towns = new List<Town>();

        foreach (var attributeElement in root
                     .GetProperty("features")
                     .EnumerateArray())
        {
            var attribute = attributeElement.GetProperty("attributes");

            var town = new Town
            {
                // ReSharper disable StringLiteralTypo
                Name = attribute.SafeGetString("place15nm", Constants.LocationNameMaxLength),
                County = attribute.SafeGetString("cty15nm", Constants.CountyMaxLength),
                LocalAuthorityName = attribute.SafeGetString("ctyltnm", Constants.CountyMaxLength),
                Latitude = attribute.SafeGetDecimal("lat"),
                Longitude = attribute.SafeGetDecimal("long")
                // ReSharper restore StringLiteralTypo
            };

            towns.Add(town);
        }

        return (towns, exceededTransferLimit);
    }
}