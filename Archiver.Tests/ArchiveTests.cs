using System.Diagnostics;

namespace Archiver.Tests;

public class ArchiveTests : IDisposable
{
    protected readonly string TestDir;
    protected readonly string TestArchive;
    protected readonly string TestFile;
    protected readonly string TestFolder;

    protected ArchiveTests()
    {
        TestDir = Path.Combine(Path.GetTempPath(), "ArchiverTests_" + Guid.NewGuid());
        Directory.CreateDirectory(TestDir);
        
        TestArchive = Path.Combine(TestDir, "test.zip");
        TestFile = Path.Combine(TestDir, "testfile.txt");
        TestFolder = Path.Combine(TestDir, "testfolder");

        File.WriteAllText(TestFile, "Test content");

        Directory.CreateDirectory(TestFolder);
        File.WriteAllText(Path.Combine(TestFolder, "insidefile.txt"), "Inside file content");
    }

    public void Dispose()
    { 
        Directory.Delete(TestDir, true);
        GC.SuppressFinalize(this);
    }
}