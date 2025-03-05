using Archiver.Services;
using ICSharpCode.SharpZipLib.Zip;

namespace Archiver.Tests;

public class CreateArchiveServiceTests : ArchiveTests
{
    [Fact]
    public void CreateArchive_CreatesValidZipFile()
    {
        const int compressionLevel = 5;
        const string password = "testpass";

        CreateArchiveService.CreateArchive(TestFolder, TestArchive, compressionLevel, password);

        Assert.True(File.Exists(TestArchive));
        
        using var zipFile = new ZipFile(TestArchive);
        zipFile.Password = password;
        Assert.True(zipFile.Count > 0);
    }

    [Fact]
    public void CreateArchive_WithSingleFile_AddsFileCorrectly()
    {
        CreateArchiveService.CreateArchive(TestFile, TestArchive, 0, null);

        using var zipFile = new ZipFile(TestArchive);
        Assert.Equal(1, zipFile.Count);
        Assert.Contains(zipFile.Cast<ZipEntry>(), 
            entry => entry.Name.EndsWith("testfile.txt"));
    }
}