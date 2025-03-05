using Spectre.Console;

namespace Archiver.Services;

public static class ArchiveCreatorService
{
    public static void CreateArchive()
    {
        var path = GetPathService.GetPath();
        var archivePath = GetPathService.GetNewArchivePath();
        var compressionLevel = ArchiveHelperService.GetCompressionLevel();
        var password = ArchiveHelperService.GetPassword();

        CreateArchiveService.CreateArchive(path, archivePath, compressionLevel, password);
        
        AnsiConsole.MarkupLine($"[green]Archiwum zostało utworzone pod ścieżką {archivePath}[/]");
    }
}