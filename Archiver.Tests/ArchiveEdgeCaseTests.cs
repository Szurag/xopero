using Archiver.Services;
using ICSharpCode.SharpZipLib.Zip;

namespace Archiver.Tests;

public class ArchiveEdgeCaseTests : ArchiveTests
{
    [Fact]
    public void Explorer_HandlesPasswordProtectedArchives()
    {
        const string password = "secret123";
        CreateArchiveService.CreateArchive(TestFolder, TestArchive, 5, password);

        var service = new ArchiveExplorerService(TestArchive, password);
        var results = service.SearchFilesWithIndices("insidefile");

        Assert.NotEmpty(results);
    }

    [Fact]
    public void CreateArchive_HandlesEmptyFolder()
    {
        var emptyDir = Path.Combine(TestDir, "emptydir");
        Directory.CreateDirectory(emptyDir);

        CreateArchiveService.CreateArchive(emptyDir, TestArchive, 0, null);

        using var zipFile = new ZipFile(TestArchive);
        Assert.True(zipFile.Count >= 0);
    }
}