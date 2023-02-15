using System.IO.Compression;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders;
public class ZipArchiveBuilder
{
    public Stream Build(
        string entryName = null,
        Stream content = null)
    {
        byte[] bytes = null;

        if (content != null)
        {
            using var ms = new MemoryStream();
            content.CopyTo(ms);
            bytes = ms.ToArray();
        }

        return Build(entryName, bytes);
    }

    public Stream Build(
        string entryName = "test.csv",
        byte[] content = null)
    {
        content ??= new byte[] {1, 2, 3, 4};

        var archiveStream = new MemoryStream();

        using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
        {
            WriteZipArchiveEntry(archive,
                    entryName,
                    content)
                .GetAwaiter().GetResult();
        }

        archiveStream.Seek(0, SeekOrigin.Begin);

        return archiveStream;
    }

    private static async Task WriteZipArchiveEntry(ZipArchive archive, string entryName, byte[] content)
    {
        var zipArchiveEntry = archive.CreateEntry(entryName, CompressionLevel.Fastest);
        await using var zipStream = zipArchiveEntry.Open();
        await zipStream.WriteAsync(content, 0, content.Length);
    }
}
