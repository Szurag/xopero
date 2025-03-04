using ICSharpCode.SharpZipLib.Zip;

namespace Szyfrator.Services;

public abstract class CreateArchiveService
{
    public static void CreateArchive()
    {
        var path = GetPathService.GetPath("Podaj path pliku / folderu, który chcesz spakować");
        var archivePath = GetPathService.GetPath("Podaj path archiwum");

        var compressionLevel = AskForCompressionLevel();

        var password = AskForPassword();

        using var fs = File.Create(archivePath);

        using var zipStream = new ZipOutputStream(fs);
        zipStream.SetLevel(Convert.ToInt32(compressionLevel));

        if (password != "")
        {
            zipStream.Password = password;
        }

        if (Directory.Exists(path))
        {
            AddDirectoryToArchive(zipStream, path, "");
        }
        else if (File.Exists(path))
        {
            AddFileToArchive(zipStream, path, "");
        }

        zipStream.CloseEntry();
    }

    public static string? AskForPassword()
    {
        Console.WriteLine("Podaj hasło do archiwum");
        Console.WriteLine("Jeżeli brak hasła, naciśnij Enter");
        return Console.ReadLine();
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

    public static string? AskForCompressionLevel()
    {
        Console.WriteLine("Wybierz poziom kompresji:");
        Console.WriteLine("1 - 9");

        var compressionLevel = Console.ReadLine();

        return compressionLevel;
    }
}