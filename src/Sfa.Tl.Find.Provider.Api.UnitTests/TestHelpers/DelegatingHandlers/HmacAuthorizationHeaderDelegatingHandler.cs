using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.DelegatingHandlers;

public class HmacAuthorizationHeaderDelegatingHandler : DelegatingHandler
{
    private readonly ApiSettings _apiSettings;

    public HmacAuthorizationHeaderDelegatingHandler(
        IOptions<ApiSettings> apiOptions)
    {
        _apiSettings = apiOptions?.Value ?? throw new ArgumentNullException(nameof(apiOptions));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = await request.GetHmacHeader(_apiSettings.AppId, _apiSettings.ApiKey);

        var response = await base.SendAsync(request, cancellationToken);

        return response;
    }
}