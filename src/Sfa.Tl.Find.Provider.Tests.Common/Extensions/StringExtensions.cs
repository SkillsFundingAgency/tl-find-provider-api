namespace Sfa.Tl.Find.Provider.Tests.Common.Extensions;
public static class StringExtensions
{
    public static async Task<Stream> ToStream(this string content)
    {
        var stream = new MemoryStream();

        await using var writer = new StreamWriter(stream, leaveOpen: true);
        await writer.WriteAsync(content);
        await writer.FlushAsync();
        stream.Position = 0;

        return stream;
    }
}
