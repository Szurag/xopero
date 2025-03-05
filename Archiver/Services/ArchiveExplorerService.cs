using Archiver.Exceptions;
using Archiver.Models;
using ICSharpCode.SharpZipLib.Zip;
using Spectre.Console;

namespace Archiver.Services;

public class ArchiveExplorerService
{
    private List<ZipItem> _zipItems = [];
    private readonly string? _password;
    private readonly string _archivePath;

    public ArchiveExplorerService(string archivePath, string? password)
    {
        _password = password;
        _archivePath = archivePath;
        MapArchive();
    }

    public void RemoveFileFromArchive(string index)
    {
        var selectedZipItem = GetZipItemByIndex(_zipItems, index);

        if (selectedZipItem == null)
        {
            throw new IncorrectIndexException();
        }

        var tempArchivePath = Path.GetTempFileName();

        using (var fs = new FileStream(_archivePath, FileMode.Open, FileAccess.Read))
        using (var zipInputStream = new ZipInputStream(fs))
        using (var tempFs = new FileStream(tempArchivePath, FileMode.Create, FileAccess.Write))
        using (var zipOutputStream = new ZipOutputStream(tempFs))
        {
            if (!string.IsNullOrEmpty(_password))
            {
                zipInputStream.Password = _password;
            }

            ZipEntry entry;
            while ((entry = zipInputStream.GetNextEntry()) != null)
            {
                zipOutputStream.Password = _password;

                if (selectedZipItem.FullPath != null &&
                    entry.Name.StartsWith(selectedZipItem.FullPath, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                zipOutputStream.PutNextEntry(new ZipEntry(entry.Name));
                zipInputStream.CopyTo(zipOutputStream);
                zipOutputStream.CloseEntry();
            }
        }

        File.Delete(_archivePath);
        File.Move(tempArchivePath, _archivePath);
        MapArchive();
    }

    public void AddFileToArchive(string path, int compressionLevel, string index)
    {
        var selectedZipItem = GetZipItemByIndex(_zipItems, index);

        if (selectedZipItem == null)
        {
            throw new IncorrectIndexException();
        }

        if (!selectedZipItem.IsDirectory)
        {
            throw new CannotAddFileToFileException();
        }

        var tempArchivePath = Path.GetTempFileName();

        using (var fs = new FileStream(_archivePath, FileMode.Open, FileAccess.Read))
        using (var zipInputStream = new ZipInputStream(fs))
        using (var tempFs = new FileStream(tempArchivePath, FileMode.Create, FileAccess.Write))
        using (var zipOutputStream = new ZipOutputStream(tempFs))
        {
            zipInputStream.Password = _password;

            zipOutputStream.SetLevel(Convert.ToInt32(compressionLevel));
            zipOutputStream.Password = _password;

            ZipEntry entry;
            while ((entry = zipInputStream.GetNextEntry()) != null)
            {
                zipOutputStream.PutNextEntry(new ZipEntry(entry.Name));
                zipInputStream.CopyTo(zipOutputStream);
                zipOutputStream.CloseEntry();
            }

            if (Directory.Exists(path))
            {
                CreateArchiveService.AddDirectoryToArchive(zipOutputStream, path, selectedZipItem.FullPath ?? "");
            }

            if (File.Exists(path))
            {
                CreateArchiveService.AddFileToArchive(zipOutputStream, path, selectedZipItem.FullPath ?? "");
            }
        }

        File.Delete(_archivePath);
        File.Move(tempArchivePath, _archivePath);

        MapArchive();
    }

    private static ZipItem? GetZipItemByIndex(List<ZipItem> zipItems, string? index)
    {
        var parts = index?.Split('.');
        var current = zipItems;

        if (parts == null) return null;
        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            var partIndex = Convert.ToInt32(part);

            if (partIndex < 0 || partIndex >= current.Count) return null;

            var item = current[partIndex];

            if (i == parts.Length - 1)
            {
                return item;
            }

            if (!item.IsDirectory) return null;

            current = item.Children;
        }

        return null;
    }

    private void MapArchive()
    {
        var fileStream = File.OpenRead(_archivePath);

        using var zipInputStream = new ZipInputStream(fileStream);
        zipInputStream.Password = _password;

        _zipItems = [];

        ZipEntry entry;

        while ((entry = zipInputStream.GetNextEntry()) != null)
        {
            var zipItem = new ZipItem
            {
                Name = entry.Name,
                IsDirectory = entry.IsDirectory,
                IsPassword = entry.IsCrypted,
                FullPath = entry.Name
            };

            AddEntryToModel(_zipItems, zipItem);
        }

        fileStream.Close();
    }

    private static void AddEntryToModel(List<ZipItem> items, ZipItem entry)
    {
        var parts = entry.Name?.Split('/');
        var current = items;

        if (parts == null) return;
        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];

            if (part == "") continue;

            var existing = current.FirstOrDefault(x => x.Name == part);

            if (existing == null)
            {
                var newItem = new ZipItem
                {
                    Name = part,
                    IsDirectory = (i < parts.Length - 1) || entry.IsDirectory,
                    FullPath = entry.FullPath
                };

                current.Add(newItem);
                current = newItem.Children;
            }
            else
            {
                current = existing.Children;
            }
        }
    }

    public List<(ZipItem, string)> SearchFilesWithIndices(string searchTerm)
    {
        var results = new List<(ZipItem, string)>();
        SearchFilesRecursiveWithIndices(_zipItems, searchTerm, results, "");
        return results;
    }

    private static void SearchFilesRecursiveWithIndices(
        List<ZipItem> items,
        string searchTerm,
        List<(ZipItem, string)> results,
        string parentIndex)
    {
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var currentIndex = parentIndex == "" ? i.ToString() : $"{parentIndex}.{i}";

            if (item.Name != null && item.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            {
                results.Add((item, currentIndex));
            }

            if (item.IsDirectory)
            {
                SearchFilesRecursiveWithIndices(item.Children, searchTerm, results, currentIndex);
            }
        }
    }

    public void DisplayTree()
    {
        DisplayRecursive(_zipItems, 0, "");
    }

    private static void DisplayRecursive(List<ZipItem> items, int level, string parentIndex)
    {
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var currentIndex = parentIndex == "" ? i.ToString() : $"{parentIndex}.{i}";
            var icon = item.IsDirectory ? "📁" : "📄";
            var paddingSpaces = new string(' ', level * 2);

            AnsiConsole.MarkupLine(
                $"{paddingSpaces}{icon} [{(item.IsDirectory ? "blue" : "green")}]{currentIndex})[/] {item.Name}{(item.IsPassword ? " 🔒" : "")}");

            if (item.IsDirectory)
            {
                DisplayRecursive(item.Children, level + 1, currentIndex);
            }
        }
    }
}