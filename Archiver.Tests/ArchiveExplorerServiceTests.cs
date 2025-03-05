using Archiver.Exceptions;
using Archiver.Services;

namespace Archiver.Tests;

public class ArchiveExplorerServiceTests : ArchiveTests
{
    private ArchiveExplorerService CreateTestArchiveWithFiles()
    {
        CreateArchiveService.CreateArchive(TestFolder, TestArchive, 0, null);
        return new ArchiveExplorerService(TestArchive, null);
    }

    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        CreateArchiveService.CreateArchive(TestFolder, TestArchive, 0, null);

        var service = new ArchiveExplorerService(TestArchive, null);

        Assert.NotNull(service);
    }

    [Fact]
    public void SearchFilesWithIndices_FindsExistingFile()
    {
        var service = CreateTestArchiveWithFiles();

        var results = service.SearchFilesWithIndices("insidefile");

        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.Item1.Name == "insidefile.txt");
    }

    [Fact]
    public void RemoveFileFromArchive_RemovesSpecifiedFile()
    {
        var service = CreateTestArchiveWithFiles();
        var searchResults = service.SearchFilesWithIndices("insidefile");
        Assert.NotEmpty(searchResults);
        var indexToRemove = searchResults[0].Item2;

        service.RemoveFileFromArchive(indexToRemove);
        
        var newResults = service.SearchFilesWithIndices("insidefile");
        Assert.Empty(newResults);
    }

    [Fact]
    public void AddFileToArchive_AddsFileToFolder()
    {
        var service = CreateTestArchiveWithFiles();
        var newFilePath = Path.Combine(TestDir, "newfile.txt");
        File.WriteAllText(newFilePath, "New file content");

        service.AddFileToArchive(newFilePath, 0, "0");

        var searchResults = service.SearchFilesWithIndices("newfile");
        Assert.NotEmpty(searchResults);
    }

    [Fact]
    public void AddFileToArchive_ThrowsException_WhenIndexIsInvalid()
    {
        var service = CreateTestArchiveWithFiles();

        Assert.Throws<IncorrectIndexException>(() => 
            service.AddFileToArchive(TestFile, 0, "999.999"));
    }
}