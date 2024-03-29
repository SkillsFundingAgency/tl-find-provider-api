﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Filters;

public class HmacAuthorizationHeaderBuilder
{
    private string _appId;
    private string _apiKey;
    private HttpMethod _method;
    private string _nonce;
    private string _requestTimestamp;
    private string _uri;
    private Stream _body;

    private readonly IHeaderDictionary _headers;

    private const string AuthorizationHeaderName = "Authorization";

    public HmacAuthorizationHeaderBuilder()
    {
        _headers = new HeaderDictionary();
    }

    // ReSharper disable once UnusedMember.Global
    public HmacAuthorizationHeaderBuilder RemoveHeader()
    {
        if (_headers.ContainsKey(AuthorizationHeaderName))
        {
            _headers.Remove(AuthorizationHeaderName);
        }

        return this;
    }

    public HmacAuthorizationHeaderBuilder WithInvalidHeader()
    {
        _headers[AuthorizationHeaderName] = new StringValues("amx invalid");
        return this;
    }

    public HmacAuthorizationHeaderBuilder WithAppId(string appId)
    {
        _appId = appId;
        return this;
    }

    public HmacAuthorizationHeaderBuilder WithApiKey(string apiKey)
    {
        _apiKey = apiKey;
        return this;
    }

    public HmacAuthorizationHeaderBuilder WithMethod(HttpMethod method)
    {
        _method = method;
        return this;
    }
    public HmacAuthorizationHeaderBuilder WithNonce(string nonce)
    {
        _nonce = nonce;
        return this;
    }

    public HmacAuthorizationHeaderBuilder WithRequestTimestamp(string requestTimestamp)
    {
        _requestTimestamp = requestTimestamp;
        return this;
    }

    public HmacAuthorizationHeaderBuilder WithUri(string uri)
    {
        _uri = uri;
        return this;
    }

    public HmacAuthorizationHeaderBuilder WithBody(Stream body)
    {
        _body = body;
        return this;
    }

    public IHeaderDictionary Build()
    {
        if (_headers.ContainsKey(AuthorizationHeaderName))
        {
            return _headers;
        }

        var requestTimestamp = _requestTimestamp 
                               ?? Convert.ToUInt64(
                                       (DateTime.UtcNow - 
                                        new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc))
                                       .TotalSeconds)
                                   .ToString();

        var nonce = _nonce ?? Guid.NewGuid().ToString("N");

        return _uri.GetHmacHeader(_method.Method, _body, _appId, _apiKey, nonce, requestTimestamp)
            .GetAwaiter()
            .GetResult()
            .ConvertToDictionary();
    }
}