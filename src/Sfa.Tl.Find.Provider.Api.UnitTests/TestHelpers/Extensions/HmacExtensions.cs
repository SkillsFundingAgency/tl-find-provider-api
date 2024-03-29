﻿using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;

public static class HmacExtensions
{
    public static async Task<AuthenticationHeaderValue> GetHmacHeader(
        this HttpRequestMessage request,
        string appId,
        string apiKey)
    {
        var requestUri = request.RequestUri!.AbsoluteUri.ToLower();
        var requestHttpMethod = request.Method.Method;

        var requestBody = request.Content != null
            ? await request.Content.ReadAsStreamAsync()
            : null;

        var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
        var timeSpan = DateTime.UtcNow - epochStart;
        var requestTimestamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

        var nonce = Guid.NewGuid().ToString("N");

        return await requestUri.GetHmacHeader(requestHttpMethod, requestBody, appId, apiKey, nonce, requestTimestamp);
    }

    public static async Task<AuthenticationHeaderValue> GetHmacHeader(
        this string requestUri,
        string requestHttpMethod,
        Stream requestBody,
        string appId,
        string apiKey,
        string nonce,
        string requestTimestamp)
    {
        string requestContentBase64String = null;
        if (requestBody != null)
        {
            using var md5 = MD5.Create();
            var requestContentHash = await md5.ComputeHashAsync(requestBody);
            requestContentBase64String = Convert.ToBase64String(requestContentHash);
        }

        var signatureRawData = $"{appId}{requestHttpMethod}{requestUri.ToLower()}{requestTimestamp}{nonce}{requestContentBase64String}";

        var secretKeyBytes = Encoding.ASCII.GetBytes(apiKey);
        var signature = Encoding.ASCII.GetBytes(signatureRawData);

        using var hmac = new HMACSHA256(secretKeyBytes);
        var signatureBytes = hmac.ComputeHash(signature);
        var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

        var header = new AuthenticationHeaderValue("amx", $"{appId}:{requestSignatureBase64String}:{nonce}:{requestTimestamp}");

        return header;
    }

    public static IHeaderDictionary ConvertToDictionary(this AuthenticationHeaderValue authenticationHeader)
    {
        return new HeaderDictionary
        {
            { "Authorization", authenticationHeader.ToString() }
        };
    }
}