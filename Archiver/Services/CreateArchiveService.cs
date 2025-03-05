using ICSharpCode.SharpZipLib.Zip;

namespace Archiver.Services;

public static class CreateArchiveService
{
    public static void CreateArchive(string path, string archivePath, int compressionLevel, string? password)
    {
        using var fs = File.Create(archivePath);
        using var zipStream = new ZipOutputStream(fs);
        SetCompressionLevel(zipStream, compressionLevel);
        SetPassword(zipStream, password);

        AddFilesToArchive(zipStream, path);
        zipStream.Close();
    }

    private static void AddFilesToArchive(ZipOutputStream zipStream, string path)
    {
        if (Directory.Exists(path))
        {
            AddDirectoryToArchive(zipStream, path, "");
        }
        
        if (File.Exists(path))
        {
            AddFileToArchive(zipStream, path, "");
        }
    }

    private static void SetPassword(ZipOutputStream zipStream, string? password)
    {
        if (password != "")
        {
            zipStream.Password = password;
        }
    }

    public static void AddFileToArchive(ZipOutputStream zipStream, string filePath, string parentDirectory)
    {
        var entryName = Path.Combine(parentDirectory, Path.GetFileName(filePath));
        var entry = new ZipEntry(entryName)
        {
            DateTime = DateTime.Now
        };

        zipStream.PutNextEntry(entry);

        using var fsInput = File.OpenRead(filePath);
        fsInput.CopyTo(zipStream);
    }

    public static void AddDirectoryToArchive(ZipOutputStream zipStream, string directoryPath, string parentDirectory)
    {
        var directoryName = Path.Combine(parentDirectory, Path.GetFileName(directoryPath));
        var entry = new ZipEntry(directoryName + "/")
        {
            DateTime = DateTime.Now
        };

        zipStream.PutNextEntry(entry);
        zipStream.CloseEntry();

        foreach (var filePath in Directory.GetFiles(directoryPath))
        {
            AddFileToArchive(zipStream, filePath, directoryName);
        }

        foreach (var subDirectory in Directory.GetDirectories(directoryPath))
        {
            AddDirectoryToArchive(zipStream, subDirectory, directoryName);
        }
    }

    private static void SetCompressionLevel(ZipOutputStream zipOutputStream, int compressionLevel)
    {
        if (compressionLevel is > 9 or < 0)
        {
            compressionLevel = 7;
        }

        zipOutputStream.SetLevel(compressionLevel);
    }
}