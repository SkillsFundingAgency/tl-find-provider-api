using System.Text;

namespace Sfa.Tl.Find.Provider.Api.Extensions;

public static class HttpRequestExtensions
{
    public static async Task<string> GetRawBodyAsync(
        this HttpRequest request,
        Encoding encoding = null)
    {
        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        request.Body.Position = 0;

        var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);
        var body = await reader.ReadToEndAsync().ConfigureAwait(false);

        request.Body.Position = 0;

        return body;
    }

    public static async Task<byte[]> GetRawBodyBytesAsync(
        this HttpRequest request,
        Encoding encoding = null)
    {
        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        request.Body.Position = 0;

        using var memoryStream = new MemoryStream();

        await request.Body.CopyToAsync(memoryStream);

        request.Body.Position = 0;

        return memoryStream.ToArray();
    }
}

