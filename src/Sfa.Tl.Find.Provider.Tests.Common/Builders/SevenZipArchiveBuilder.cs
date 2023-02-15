using Aspose.Zip.SevenZip;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders;
public class SevenZipArchiveBuilder
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
        content ??= new byte[] { 1, 2, 3, 4 };

        var archiveStream = new MemoryStream();

        using (var archive = new SevenZipArchive())
        {
            using var ms = new MemoryStream(content);
            archive.CreateEntry(entryName, ms);
            archive.Save(archiveStream);
        }
        archiveStream.Seek(0, SeekOrigin.Begin);

        return archiveStream;
    }
}
